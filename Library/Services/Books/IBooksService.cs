using Library.Services.Books.Models;

namespace Library.Services.Books
{
    public interface IBooksService
    {
        Task<IEnumerable<BookModel>> GetBooks();

        Task CreateBook(BookModel book);
    }
}
