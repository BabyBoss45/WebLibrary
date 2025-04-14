namespace Library.Services.Books.Models
{
    public class GenreModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public GenreModel() { }

        public GenreModel(IsbndbBookModel genre)
        {
            Name = genre.Subjects[0];
        }
    }
}
