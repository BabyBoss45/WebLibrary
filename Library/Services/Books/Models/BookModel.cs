using System.ComponentModel.DataAnnotations;

namespace Library.Services.Books.Models
{
    public class BookModel
    { 
        public long Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? Date { get; set; }

        public string Summary { get; set; }
    }
}
