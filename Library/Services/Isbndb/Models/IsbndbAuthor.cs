namespace Library.Services.Isbndb.Models
{
    public class IsbndbAuthor
    {
        public int Id { get; set; }
        public string author { get; set; } 
        public List<IsbndbBookModel> books { get; set; }
    }
}
