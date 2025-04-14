using Library.Services.Books.Models;
using Library.Services.Isbndb.Models;
namespace Library.Services.Isbndb
{
    public interface IIsbndbService
    {
        Task<BookModel> GetBook(string isbn);
    }
}
