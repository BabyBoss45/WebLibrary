using Library.Services.Books;
using Library.Services.Isbndb;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static System.Reflection.Metadata.BlobBuilder;

namespace Library.Controllers
{
    public class IsbndbController : Controller
    {
        private readonly IBooksService _books;
        private readonly IIsbndbService _isbn;
        public IsbndbController(IIsbndbService isbn, IBooksService books)
        {
            _books = books;
            _isbn = isbn;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetBook([StringLength(13,MinimumLength =10)]string isbn)
        {
            if (ModelState.IsValid) 
            {
                //_books.CreateBook();
                await _isbn.GetBook(isbn);  

            }
               
            return View(isbn);

        }
    }
}
