using System.ComponentModel.DataAnnotations;

namespace Library.Services.Books.Models
{
    public class BookModel
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(512)]
        [MinLength(2)]
        [Display(Name="Book name")]
        public string Name { get; set; }

        public byte[] Photo { get; set; }
        [Display(Name = "Release date")]
        public DateTime DateRelease { get; set; }

        public string Summary { get; set; }


        public string Language { get; set; }

        public List<int> IdAuthors { get; set; }

    }
}
