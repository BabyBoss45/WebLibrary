namespace Library.Services.Isbndb
{
    public interface IIsbndbService
    {
        Task GetBook(string isbn);
    }
}
