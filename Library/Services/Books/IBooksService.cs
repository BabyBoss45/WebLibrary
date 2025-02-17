using Library.Services.Books.Models;
namespace Library.Services.Books
{
    public interface IBooksService
    {
        Task<IEnumerable<BookModel>> GetBooks();
        Task CreateBook(BookModel book);
        Task DeleteBook(BookModel book);
        Task<BookModel> GetBook(long id);
        Task AddAuthor(AuthorsModel author);
        Task AddInventory(InventoryModel inventory);
        Task AddGenre(GenreModel genre);
        Task AddGenreToBook(GenresBookModel genresBook);
        Task BookStatus(StatusModel status);
    }
}
