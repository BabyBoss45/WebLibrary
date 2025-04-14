using Dapper;
using Library.Data;
using Library.Services.Books.Models;
using System.Data;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using System.Net;

namespace Library.Services.Books
{
    // This service handles book-related operations
    public class BooksService : IBooksService
    {
        // Logger for logging errors and information
        private readonly ILogger<BooksService> _logger;

        // Database connection abstraction
        private readonly IDb _db;

        // Constructor to inject logger and database 
        public BooksService(ILogger<BooksService> logger, IDb db)
        {
            _logger = logger;
            _db = db;
        }

        // Getting a list of books from the database
        public async Task<IEnumerable<BookModel>> GetBooks()
        {
            try
            {
                // Open a database connection
                using (var con = await _db.ConnectAsync())
                {
                    // Query the Book table and return the result as a list of BookModel
                    return await con.QueryAsync<BookModel>(@"
                SELECT 
                    ID AS Id, 
                    NAME AS Name, 
                    PHOTO AS Photo, 
                    PHOTOURL AS PhotoLink,  -- Map PHOTOURL to PhotoLink
                    DATERELEASE AS DateRelease, 
                    SUMMARY AS Summary, 
                    LANGUAGE AS Language
                FROM BOOK");
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the query
                _logger.LogError(ex, "Get books error");
            }

            // Return an empty list if an error occurs
            return Enumerable.Empty<BookModel>();
        }

        public async Task CreateBook(BookModel book)
        {
            try
            {
                //connecting to db
                using (var con = await _db.ConnectAsync())
                {
                    //creating transacion
                    using (var trn = con.BeginTransaction())
                    {
                        try
                        {  // creating dynamic parameter for db
                            var param = new DynamicParameters(book);
                            // addimg the id 
                            param.Add(name: nameof(book.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                            //main insert of the data into db to book table
                            await con.ExecuteAsync($@"INSERT INTO BOOK (NAME, DATERELEASE, SUMMARY, LANGUAGE, PHOTO, PHOTOURL)
                    VALUES ({_db.Param(nameof(book.Name))}, {_db.Param(nameof(book.DateRelease))}, {_db.Param(nameof(book.Summary))}, {_db.Param(nameof(book.Language))},
                    {_db.Param(nameof(book.Photo))},{_db.Param(nameof(book.PhotoLink))} )
                    {_db.Returning(nameof(book.Id))}", param, transaction: trn);
                            //geting book id 
                            book.Id = param.Get<long>(nameof(book.Id));

                            _logger.LogDebug("Create book {Id}", book.Id);
                            // checking if book have authors
                            if (book.Authors != null)
                            {   // looping to add each author
                                foreach (var author in book.Authors)
                                {
                                    // cheking if author is in db
                                    if (author.Id == 0)
                                    {  // looking for an author 
                                        var id = await FindAuthor(author.Name);
                                        if (id.HasValue)
                                        {   //giving id of author from db
                                            author.Id = id.Value;
                                        }
                                        else
                                        {
                                            //creating new one
                                            await AddAuthor(author);
                                        }
                                    }
                                    //linking the id of book with id of author
                                    await con.ExecuteAsync($@"INSERT INTO AUTHORSBOOKS (IDAUTHOR,  IDBOOK)
                                    VALUES({_db.Param(nameof(author))}, {_db.Param(nameof(book.Id))});
                                    ", new { author = author.Id, book.Id }, transaction: trn);

                                }
                            }
                            //checking if book have genres
                            if (book.Genres != null)
                            {
                                //looping for each genre
                                foreach (var genre in book.Genres)
                                {
                                    //chekcing if genre is db
                                    if (genre.Id == 0)
                                    {
                                        //looking for genre
                                        var id = await FindGenre(genre.Name);
                                        if (id.HasValue)
                                        {
                                            // giving id from db to genre
                                            genre.Id = id.Value;
                                        }
                                        else
                                        {
                                            //adding new genre
                                            await AddGenre(genre);
                                        }
                                    }
                                    //linking the id of book with id of genre
                                    await con.ExecuteAsync($@"INSERT INTO BOOKGENRES (IDGENRE,  IDBOOK)
                                    VALUES({_db.Param(nameof(genre))}, {_db.Param(nameof(book.Id))});
                                    ", new { genre = genre.Id, book.Id }, transaction: trn);
                                }
                            }

                            //adding inventory number 
                            try
                            {

                                // Create a new InventoryModel object to store information about a book in the library's inventory
                                var lib = new InventoryModel();

                                // Set the ID of the book being added to the inventory
                                lib.IdBook = book.Id;

                                // Set the name of the library where the book is stored
                                lib.LibraryName = book.LibName;

                                // Create a set of dynamic parameters using the lib object (this is for passing values into the SQL query)
                                var paramLib = new DynamicParameters(lib);

                                // Add an output parameter for the 'Id' field, which will store the new record's ID after the insert
                                paramLib.Add(name: nameof(lib.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                                // Run the SQL command to insert the new record into the INVENTORY table
                                await con.ExecuteAsync($@"
                                INSERT INTO INVENTORY (NAMELIBRARY, IDBOOK)
                                VALUES ({_db.Param(nameof(lib.LibraryName))}, {_db.Param(nameof(lib.IdBook))})
                                {_db.Returning(nameof(lib.Id))} -- This part returns the new inventory record's ID
                                ", paramLib, transaction: trn);

                                // Retrieve the generated ID from the output parameter and store it back into the lib object
                                lib.Id = paramLib.Get<long>(nameof(lib.Id));

                                _logger.LogDebug("  Inventory number added {Id}", lib.Id);

                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Add Invnetory number  error {Json}", JsonConvert.SerializeObject(book));
                            }

                            //end of transaction 
                            trn.Commit();
                        }
                        catch (Exception ex)
                        {
                            //logging errors
                            _logger.LogError(ex, "Create book error {Json}", JsonConvert.SerializeObject(book));
                            trn.Rollback();

                        }

                    }
                }
            }
            catch (Exception ex)
            { //logging errors
                _logger.LogError(ex, "Create book error {Json}", JsonConvert.SerializeObject(book));
            }
        }
        // Deletes a book record from the database using the provided BookModel
        public async Task DeleteBook(BookModel book,string reason)
        {
            try
            {
                // Open a connection to the database
                using (var con = await _db.ConnectAsync())
                {
                    // Create dynamic parameters from the book object

                    // Add an output parameter for the book ID (not really needed here for DELETE, but included anyway)
                    var param = new DynamicParameters();
                    param.Add("Id", book.Id, DbType.Int64);

                    // Execute the DELETE statement and pass in the parameters
                    await con.ExecuteAsync(@"DELETE FROM BOOK WHERE ID = @Id", param);

                    // Get the deleted book's ID from the output parameters (optional / unnecessary for DELETE)
                    book.Id = param.Get<long>(nameof(book.Id));

                    // Log the deletion of the book
                    _logger.LogDebug("Delete book {Id}", book.Id);
                }
            }
            catch (Exception ex)
            {
                // Log any error that occurs along with the serialized book data
                _logger.LogError(ex, "Delete book error {Json}", JsonConvert.SerializeObject(book));
            }
        }

        // Retrieves a single book by its ID, including its associated authors
        public async Task<BookModel> GetBook(long id)
        {
            try
            {
                // Open a connection to the database
                using (var con = await _db.ConnectAsync())
                {
                    // Query the Book table for a book with the specified ID
                    var book = await con.QueryFirstOrDefaultAsync<BookModel>(
                        $@"SELECT  
                    NAME AS Name, 
                    PHOTO AS Photo, 
                    PHOTOURL AS PhotoLink,  -- Map PHOTOURL to PhotoLink
                    DATERELEASE AS DateRelease, 
                    SUMMARY AS Summary, 
                    LANGUAGE AS Language
                    FROM Book WHERE id = {_db.Param(nameof(id))}", new { id });

                    // If a book was found, retrieve its associated authors
                    if (book != null)
                    {
                        book.Authors = (await con.QueryAsync<AuthorsModel>(
                            $@"SELECT a.Id, a.Name 
                        FROM AUTHORSBOOKS ab 
                        LEFT JOIN HBAUTHOR a ON a.ID = ab.IDAUTHOR
                        WHERE ab.IDBOOK = {_db.Param(nameof(id))}", new { id })).AsList();

                        book.Genres = (await con.QueryAsync<GenreModel>($@"SELECT a.Id, a.NAMEGENRE AS Name 
                        FROM BOOKGENRES ab 
                        LEFT JOIN HBGENRE a ON a.ID = ab.IDGENRE
                        WHERE ab.IDBOOK = {_db.Param(nameof(id))}", new { id })).AsList();

                        book.Id = id;

                        book.LibName = await con.QuerySingleOrDefaultAsync<string>($@"SELECT NAMELIBRARY 
                        FROM INVENTORY 
                        WHERE IDBOOK = {_db.Param(nameof(book.Id))}",
                        new { book.Id });

                        // Return the book (with or without authors)
                    }
                    return book;
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the process
                _logger.LogError(ex, "Get books error");
            }

            // Return null if an error occurred or the book was not found
            return null;
        }

        // require a name of author and searching for it
        public async Task<long?> FindAuthor(string name)
        {
            try
            {
                //connecting to database
                using (var con = await _db.ConnectAsync())
                {
                    // return the id of an author 
                    return await con.QueryFirstOrDefaultAsync<long?>(
                        $@"SELECT Id  FROM HBAUTHOR WHERE name = {_db.Param(nameof(name))}", new { name });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get author error");
            }
            //null if author was not found
            return null;
        }

        public async Task UpdateBook(BookModel book)
        {

        }

        //requiere the model of author
        public async Task AddAuthor(AuthorsModel author)
        {
            try
            {   //connecting to database 
                using (var con = await _db.ConnectAsync())
                {
                    //creating dynamic parameter for insetion
                    var param = new DynamicParameters(author);
                    //adding the id of author
                    param.Add(name: nameof(author.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);
                    //inserting the auhtor name into the database
                    await con.ExecuteAsync($@"INSERT INTO HBAUTHOR (NAME)
                    VALUES ({_db.Param(nameof(author.Name))}) {_db.Returning(nameof(author.Id))}", param);
                    author.Id = param.Get<long>(nameof(author.Id));
                    //loging author into logger
                    _logger.LogDebug("Add author {Id}", author.Id);
                }
            }
            catch (Exception ex)
            {
                //handle error about not adding author
                _logger.LogError(ex, "Adding author error {Json}", JsonConvert.SerializeObject(author));
            }
        }

        public async Task<long?> FindGenre(string name)
        {
            try
            {
                //connecting to database
                using (var con = await _db.ConnectAsync())
                {
                    // return the id of genre 
                    return await con.QueryFirstOrDefaultAsync<long?>(
                        $@"SELECT Id  FROM HBGENRE WHERE namegenre = {_db.Param(nameof(name))}", new { name });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get genre error");
            }
            //null if genre was not found
            return null;
        }
        // require Genre model 
        public async Task AddGenre(GenreModel genre)
        {
            try
            {
                // an asynchronous connection to the database
                using (var con = await _db.ConnectAsync())
                {
                    // Create a set of dynamic parameters based on the genre object
                    var param = new DynamicParameters(genre);

                    // Define the output parameter to capture the generated genre ID
                    param.Add(name: nameof(genre.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                    // Execute the SQL INSERT with genre name and creates Id
                    await con.ExecuteAsync($@"
                INSERT INTO HBGENRE (NAMEGENRE)
                VALUES ({_db.Param(nameof(genre.Name))}) 
                {_db.Returning(nameof(genre.Id))}", param);

                    // assign the new generated ID back to the genre object
                    genre.Id = param.Get<long>(nameof(genre.Id));

                    // Log the successful addition with the new genre ID
                    _logger.LogDebug("Add Genre {Id}", genre.Id);
                }
            }
            catch (Exception ex)
            {
                // Log the error along with the genre object for debugging
                _logger.LogError(ex, "Adding genre error {Json}", JsonConvert.SerializeObject(genre));
            }
        }
        // Associates one or more genres with a specific book by inserting into the BOOKGENRES table
        public async Task AddGenreToBook(GenresBookModel genresBook)
        {
            try
            {
                // Open a connection to the database
                using (var con = await _db.ConnectAsync())
                {
                    // Loop through each genre ID in the list
                    foreach (var genre in genresBook.IdGenre)
                    {
                        // Insert the book-genre relationship into the BOOKGENRES table
                        await con.ExecuteAsync($@"
                    INSERT INTO BOOKGENRES (IDBOOK, IDGENRE)
                    VALUES ({_db.Param(nameof(genresBook.IdBook))}, {_db.Param(nameof(genre))})",
                            new { genresBook.IdBook, genre });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error along with the JSON representation of the input
                _logger.LogError(ex, "Add genre error {Json}", JsonConvert.SerializeObject(genresBook));
            }
        }


        // Adds a new book status to the database (e.g., Available, Borrowed, Damaged)
        public async Task BookStatus(StatusModel status)
        {
            try
            {
                // Open a connection to the database
                using (var con = await _db.ConnectAsync())
                {
                    // Prepare dynamic parameters from the StatusModel
                    var param = new DynamicParameters(status);

                    // Add an output parameter to capture the generated ID
                    param.Add(name: nameof(status.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                    // Insert the new status into the HBBOOKSTATUS table and return the new ID
                    await con.ExecuteAsync($@"
                INSERT INTO HBBOOKSTATUS (NAMESTATUS)
                VALUES ({_db.Param(nameof(status.Name))})
                {_db.Returning(nameof(status.Id))}", param);

                    // Retrieve the newly inserted ID
                    status.Id = param.Get<int>(nameof(status.Id));

                    // Log the newly added status ID
                    _logger.LogDebug("Add status {Id}", status.Id);
                }
            }
            catch (Exception ex)
            {
                // Log any error along with the serialized input for debugging
                _logger.LogError(ex, "Adding status error {Json}", JsonConvert.SerializeObject(status));
            }
        }

    }
}
