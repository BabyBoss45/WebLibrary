using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web;

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
        
        public string LibName { get; set; }
        public List<AuthorsModel> Authors { get; set; }

        public List<GenreModel> Genres { get; set; }

        public BookModel() { }
        public BookModel(IsbndbBookModel isnb)
        { 
            // asigning varibles from api to model variables
            Id = isnb.Id;
            Name = isnb.TitleLong;
            Name = isnb.Title;
            PhotoLink = isnb.Image;
            DateRelease = isnb.DatePublished;
            Summary = isnb.Synopsis;
            Language = isnb.Language;
            Authors = new List<AuthorsModel>(); // asigning authors to model
            Genres  = new List<GenreModel>();
            //adding authours with author model from api
            foreach (var author in isnb.Authors)
            {
                // adding author model with name from api
                Authors.Add(new AuthorsModel {Name = author} );
            }
            //adding genres(subjects) with genre model from api
            foreach (var genre in isnb.Subjects)
            {
                //adding a name of genre to the list
                Genres.Add(new GenreModel {Name = genre} );
            }
        }

    }

}
