using Dapper;
using Library.Data;
using Library.Services.Isbndb.Models;
using System.Data;
using System.Threading;
using System.Text;
using Library.Services.Books;
using Library.Services.Books.Models;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Library.Services.Isbndb
{

    public class IsbndbService
    {
        private readonly ILogger<IsbndbService> _logger;
        private readonly IDb _db;
        public IsbndbService(ILogger<IsbndbService> logger, IDb db)
        {
            _logger = logger;
            _db = db;
        }
        public async Task<BookModel> GetBook(string isbn)
        {
            const string ApiKey = "54525_f998566a066fc1456965169e594256ef";
            const string BaseUrl = "https://api2.isbndb.com";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", ApiKey);

                    string endpoint = $"/book/{isbn}";
                    HttpResponseMessage response = await client.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        // Read and display the response content
                        string jsonString = await response.Content.ReadAsStringAsync();
                        IsbndbBookModel bookModel = JsonSerializer.Deserialize<IsbndbBookModel>(jsonString);

                        //return new BookModel(bookModel.book) //как в тест проге 

                    }
                    else
                    {
                        // Handle errors (e.g., invalid ISBN, unauthorized)
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        _logger.LogDebug($"Error: {response.StatusCode}");
                        _logger.LogDebug($"Details: {errorResponse}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"An error occurred: {ex.Message}");
            }
            return null;
        }

        public async Task GetAuthor(IsbndbBookModel bookModel, AuthorsModel authors)
        {
            int count = 0;
            foreach (var author in bookModel.Authors)
            {
                try
                {
                    authors.Name = bookModel.Authors.ElementAt<string>(count);
                    using (var con = await _db.ConnectAsync())
                    {
                        var param = new DynamicParameters(bookModel);

                        param.Add(name: nameof(authors.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                        await con.ExecuteAsync($@"INSERT INTO HBAUTHOR (NAME)
                    VALUES ({_db.Param(nameof(authors.Name))},{_db.Returning(nameof(authors.Id))}", param);
                        authors.Id = param.Get<int>(nameof(authors.Id));

                        _logger.LogDebug("Add author {Id}", authors.Id);
                    } 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Adding author error {Json}", JsonConvert.SerializeObject(author));
                }
                count++;
            }
        }
    }
}
