using Library.Services.Isbndb.Models;

namespace Library.Services.Books.Models
{
    public class AuthorsModel
    {
        public long Id { get; set; }
        public string Name { get; set; } 
        
        public AuthorsModel() { }

        public AuthorsModel(IsbndbBookModel author)
        {
            Id = author.Id;
            Name = author.Authors[0];
        }
    }
   
}
