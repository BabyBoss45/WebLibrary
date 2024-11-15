using System.ComponentModel.DataAnnotations;

namespace Library.Services.Books.Models
{
    public class BookModel
    { 
        public long Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string Name { get; set; }

        // photo
        public DateTime ReleaseDate { get; set; }

        public string Summary { get; set; }

        public DateTime? Date { get; set; } // зачем?

        public string Language { get; set; }

        public int IdAuthor { get; set; }

    }
}
