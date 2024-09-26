using Library.Services.Books;
using Library.Services.Books.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBooksService _books;
        public BooksController(IBooksService books)
        {
            _books = books;
        }   

        public async Task<IActionResult> Index()
        {
            var list = await _books.GetBooks();
            return View(list);
        }

        public IActionResult Edit()
        { 
            return View();
        }

        [HttpPost]
        public IActionResult Edit(BookModel model)
        {
            if (!ModelState.IsValid) 
            { 
                return View(model);
            }

            return View();
        }
    }
}
