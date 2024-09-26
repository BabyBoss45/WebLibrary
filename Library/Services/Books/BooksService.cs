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
                        $@"SELECT * FROM Books");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get books error");
            }

            return Enumerable.Empty<BookModel>();
        }

        public async Task CreateBook(BookModel book)
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    var param = new DynamicParameters(book);

                    param.Add(name: nameof(book.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                    await con.ExecuteAsync($@"INSERT INTO AppUsers (Name, Date, Summary)
                    VALUES ({_db.Param(nameof(book.Name))}, {_db.Param(nameof(book.Date))}, {_db.Param(nameof(book.Summary))})
                    {_db.Returning(nameof(book.Id))}", param);

                    book.Id = param.Get<long>(nameof(book.Id));

                    _logger.LogDebug("Create book {Id}", book.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create book error {Json}", JsonConvert.SerializeObject(book));
            }
        }
    }
}
