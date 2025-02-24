using Library.Services.Books;
using Library.Services.Books.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    // изменить методы 
    //[Authorize]
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
        // создание и редакт книги 
        public async Task<IActionResult> Edit(long id)
        {
            BookModel model = null;
            if (id > 0)
            {
                model = await _books.GetBook(id);
                if (model == null)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                model = new BookModel() { DateRelease = DateTime.Now };
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.Id == 0)
            {
                await _books.CreateBook(model);
            }
            else
            {
                //update
            }



            return RedirectToAction(nameof(Index));
            // return View();
        }
        public async Task<IActionResult> BooksManager()
        {
            return View();
        }
        public async Task<IActionResult> BooksManagerAddManual()
        {
            return View();
        }
    }
}
