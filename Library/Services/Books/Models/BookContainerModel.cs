namespace Library.Services.Books.Models
{
    public class BookContainerModel
    {
        public BookModel book { get; set; }
        //Должны быть листы но для костыля будут пока по одному
        public AuthorsModel authors { get; set; }
        public GenreModel genre { get; set; }
    }
}
