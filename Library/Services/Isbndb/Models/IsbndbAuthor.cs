namespace Library.Services.Isbndb.Models
{
    public class IsbndbAuthor
    {
        public string author { get; set; } 
        public List<IsbndbBookModel> books { get; set; }
    }
}
