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
        public string PhotoLink{ get; set; }
        [Display(Name = "Release date")]
        public DateTime DateRelease { get; set; }

        public string Summary { get; set; }


        public string Language { get; set; }
        
        public string LibraryName{ get; set; }
        public List<int> IdAuthors { get; set; }

        public BookModel() { }
        public BookModel(IsbndbBookModel isnb)
        { 
            Id = isnb.Id;
            Name = isnb.TitleLong;
            PhotoLink = isnb.Image;
            DateRelease = isnb.DatePublished;
            Summary = isnb.Synopsis;
            Language = isnb.Language;
        }

    }
    
}
