using Dapper;
using Library.Data;
using Library.Services.Books.Models;
using System.Data;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using Library.Services.Borrowing.Models;

namespace Library.Services.Borrowing
{
    public class BorrowingService : IBorrowingService
    {
        private readonly ILogger<BorrowingService> _logger;
        private readonly IDb _db;

        public BorrowingService(ILogger<BorrowingService> logger, IDb db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task LogBorrowing(BorrowingModel borrowing)
        {
            try
            {
                using (var con = await _db.ConnectAsync())
                {
                    var param = new DynamicParameters(borrowing);
                    await con.ExecuteAsync($@"INSERT INTO LOGINVENTORY(IDUSER, STARTDATE, ENDDATE, IDSTATUS) VALUES({_db.Param(nameof(borrowing.IdUser))}, {_db.Param(nameof(borrowing.StarDate))}
                    {_db.Param(nameof(borrowing.StarDate))},{_db.Param(nameof(borrowing.EndDate))}, {_db.Param(nameof(borrowing.IdStatus))}", param);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogBorrowing error {Json}", JsonConvert.SerializeObject(borrowing));
            }
        }
    }
}
