using Dapper;
using Library.Data;
using Library.Services.Books.Models;
using System.Data;
using System.Threading;
using System.Text;
using Newtonsoft.Json;

namespace Library.Services.Books
{
    public class BooksService : IBooksService
    {
        private readonly ILogger<BooksService> _logger;
        private readonly IDb _db;

        public BooksService(ILogger<BooksService> logger,
            IDb db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IEnumerable<BookModel>> GetBooks()
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    return await con.QueryAsync<BookModel>(
                        $@"SELECT * FROM Book");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get books error");
            }

            return Enumerable.Empty<BookModel>();
        }
        /// <summary>
        /// описать метод
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        /// 

        public async Task CreateBook(BookModel book)
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    using (var trn = con.BeginTransaction())
                    {
                        try
                        {
                            var param = new DynamicParameters(book);

                            param.Add(name: nameof(book.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                            await con.ExecuteAsync($@"INSERT INTO BOOK (NAME, DATERELEASE, SUMMARY, LANGUAGE)
                    VALUES ({_db.Param(nameof(book.Name))}, {_db.Param(nameof(book.DateRelease))}, {_db.Param(nameof(book.Summary))}, {_db.Param(nameof(book.Language))})
                    {_db.Returning(nameof(book.Id))}", param, transaction: trn);

                            book.Id = param.Get<long>(nameof(book.Id));

                            _logger.LogDebug("Create book {Id}", book.Id);
                            if (book.IdAuthors != null)
                            {
                                foreach (var author in book.IdAuthors)
                                {
                                    await con.ExecuteAsync($@"INSERT INTO AUTHORSBOOKS (IDAUTHOR,  IDBOOK)
                                    VALUES({_db.Param(nameof(author))}, {_db.Param(nameof(book.Id))}));
                                    ", new { author, book.Id }, transaction: trn);
                                }
                            }

                            trn.Commit();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Create book error {Json}", JsonConvert.SerializeObject(book));
                            trn.Rollback();

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create book error {Json}", JsonConvert.SerializeObject(book));
            }
        }
        public async Task DeleteBook(BookModel book)
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    var param = new DynamicParameters(book);

                    param.Add(name: nameof(book.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                    await con.ExecuteAsync($@"DELETE FROM BOOK WHERE ID = @ID");

                    book.Id = param.Get<long>(nameof(book.Id));

                    _logger.LogDebug("Delete book {Id}", book.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete book error {Json}", JsonConvert.SerializeObject(book));
            }
        }
        public async Task<BookModel> GetBook(long id)
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    return await con.QueryFirstOrDefaultAsync<BookModel>(
                        $@"SELECT * FROM Book WHERE id = {nameof(id)}", new {id});
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get books error");
            }

            return null;
        }

        public async Task UpdateBook(BookModel book)
        {

        }

        public async Task AddAuthor(AuthorsModel author)
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    var param = new DynamicParameters(author);

                    param.Add(name: nameof(author.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                    await con.ExecuteAsync($@"INSERT INTO HBAUTHOR (NAME)
                    VALUES ({_db.Param(nameof(author.Name))},{_db.Returning(nameof(author.Id))}", param);
                    author.Id = param.Get<int>(nameof(author.Id));

                    _logger.LogDebug("Add author {Id}", author.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Adding author error {Json}", JsonConvert.SerializeObject(author));
            }
        }

        public async Task AddInventory(InventoryModel inventory)
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    var param = new DynamicParameters(inventory);

                    param.Add(name: nameof(inventory.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                    await con.ExecuteAsync($@"INSERT INTO INVENTORY (IDBOOK, NAMELIBRARY, LIBRARYROOM)
                    VALUES ({_db.Param(nameof(inventory.IdBook))},{_db.Param(nameof(inventory.NameLibrary))},
                     {_db.Param(nameof(inventory.LybraryRoom))},{_db.Returning(nameof(inventory.Id))}", param);

                    inventory.Id = param.Get<int>(nameof(inventory.Id));

                    _logger.LogDebug("Added inventory number  {Id}", inventory.Id);


                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Adding Inventory error {Json}", JsonConvert.SerializeObject(inventory));
            }
        }
        public async Task AddGenre(GenreModel genre)
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    var param = new DynamicParameters(genre);

                    param.Add(name: nameof(genre.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                    await con.ExecuteAsync($@"INSERT INTO HBGENRE (NAMEGENRE)
                    VALUES ({_db.Param(nameof(genre.Name))}, {_db.Returning(nameof(genre.Id))}", param);

                    genre.Id = param.Get<int>(nameof(genre.Id));
                    _logger.LogDebug("Add Genre {Id}",genre.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Adding genre error {Json}", JsonConvert.SerializeObject(genre));
            }
        }
        public async Task AddGenreToBook(GenresBookModel genresBook)
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    var param = new DynamicParameters(genresBook);

                    foreach (var genre in genresBook.IdGenre)
                    {
                        await con.ExecuteAsync($@"INSERT INTO BOOKGENRES (IDBOOK, IDGENRE)
                    VALUES ({_db.Param(nameof(genresBook.IdBook))},{_db.Param(nameof(genresBook.IdGenre))}", param);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add genre error {Json}", JsonConvert.SerializeObject(genresBook));
            }
        }

        public async Task BookStatus(StatusModel status)
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    var param = new DynamicParameters(status);

                    param.Add(name: nameof(status.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                    await con.ExecuteAsync($@"INSERT INTO HBBOOKSTATUS (NAMESTATUS)
                    VALUES ({_db.Param(nameof(status.Name))}, {_db.Returning(nameof(status.Id))},", param);

                    status.Id = param.Get<int>(nameof(status.Id));

                    _logger.LogDebug("Add status {Id}",status.Id);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Adding Inventory error {Json}", JsonConvert.SerializeObject(status));
            }
        }
    }
}
