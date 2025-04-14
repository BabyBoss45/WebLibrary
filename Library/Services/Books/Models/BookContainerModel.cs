using Library.Services.Isbndb.Models;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Library.Services.Books.Models
{
    public class BookContainerModel
    {
        public BookModel book {  get; set; }
        public string  authors { get; set; }
        public string genres { get; set; }

        public IFormFile PhotoUpload { get; set; }
        public BookContainerModel() { }

    }
}
