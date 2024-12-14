using Library.Services.Borrowing.Models;

namespace Library.Services.Borrowing
{
    public interface IBorrowingService
    {
        Task LogBorrowing(BorrowingModel borrowing);
        
    }
}
