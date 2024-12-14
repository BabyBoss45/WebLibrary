using Dapper;
using Library.Data;
using Library.Services.Isbndb.Models;
using System.Data;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using Library.Services.Books;


namespace Library.Services.Isbndb
{
    //сделать как bookservice через service 

    public class IsbndbService
    {
        private readonly ILogger<IsbndbService> _logger;
        private readonly IDb _db;
        // сделать 
        public IsbndbService(ILogger<IsbndbService> logger, IDb db)
        {
            _logger = logger;
            _db = db;
        }
        public async Task GetBook(string isbn)
        {
            const string ApiKey = "54525_f998566a066fc1456965169e594256ef";
            const string BaseUrl = "https://api2.isbndb.com";
            isbn = "9781316644300";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (var con = await _db.ConnectAsync())
                    {// стоит ли использовать transaction ?
                        try
                        {
                           
                            client.BaseAddress = new Uri(BaseUrl);
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", ApiKey);

                            string endpoint = $"/book/{isbn}";
                            HttpResponseMessage response = await client.GetAsync(endpoint);
                            
                            if (response.IsSuccessStatusCode)
                            {
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                IsbndbBookModel model = JsonConvert.DeserializeObject<IsbndbBookModel>(jsonResponse);

                                Console.WriteLine("Book Details:");
                                Console.WriteLine(jsonResponse);
                            }
                            else
                            {
                                _logger.LogError(isbn, "book with this code not found");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "book with this code not found");
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error to get a book from Api");
            }
        }
    }
}
