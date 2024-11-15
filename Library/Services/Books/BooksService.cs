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
        // вопрос по сохранению id книги из api а также логика для сохранения фото
        public async Task CreateBook(BookModel book)
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    var param = new DynamicParameters(book);

                    param.Add(name: nameof(book.Id), dbType: DbType.Int64, direction: ParameterDirection.Output);

                    await con.ExecuteAsync($@"INSERT INTO BOOK (NAME, RELEASE_DATE, SUMMARY, LANGUAGE)
                    VALUES ({_db.Param(nameof(book.Name))}, {_db.Param(nameof(book.Date))}, {_db.Param(nameof(book.Summary))}, {_db.Param(nameof(book.Language))})
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
        public async Task DeleteBook(BookModel book) // можно ли вместе модели просто idBook?
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
    }
}
