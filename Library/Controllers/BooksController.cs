using Humanizer.Localisation;
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

        //main books page 
        public async Task<IActionResult> Index()
        {
            //creating a list of books 
            var list = await _books.GetBooks();

            //returning list to the page 
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
        [HttpGet]
        public async Task<IActionResult> BookInfo(long id = 0)
        {
            if (id > 0)
            {
                var book = await _books.GetBook(id);
                var bookCon = new BookContainerModel { book = book };
                if (book.Authors != null)
                {
                    bookCon.authors = string.Join(", ", book.Authors.Select(a => a.Name));
                }

                if (book.Genres != null)
                {
                    bookCon.genres = string.Join(", ", book.Genres.Select(a => a.Name));
                }
                return View(bookCon);
            }
            return View();
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
        [HttpGet]
        public async Task<IActionResult> BooksManagerAddManual(long id = 0)
        {
            if (id > 0)
            {
                var book = await _books.GetBook(id);
                var bookCon = new BookContainerModel {book = book };
                if(book.Authors != null)
                {
                    bookCon.authors = string.Join(", " , book.Authors.Select(a=> a.Name));   
                }

                if(book.Genres != null)
                {
                    bookCon.genres = string.Join(",", book.Genres.Select(a => a.Name));
                }
                return View(bookCon);
            }
            return View();
        }

        // This is an HTTP POST action method for manually adding a book via a form submission
        [HttpPost]
        public async Task<IActionResult> BooksManagerAddManual(BookContainerModel model)
        {
            // Check if the model passed from the form is valid based on validation rules
            if (ModelState.IsValid)
            {
                try
                {
                    // Convert the authors string into a list of AuthorsModel objects
                    model.book.Authors = model.authors.Split(",").Select(a => new AuthorsModel { Name = a.Trim() }).ToList();

                    // Convert the genres string into a list of GenreModel objects
                    model.book.Genres = model.genres.Split(",").Select(g => new GenreModel { Name = g.Trim() }).ToList();

                    // Save photoData  to database or file system
                    using (var memoryStream = new MemoryStream())
                    {
                        model.PhotoUpload.CopyTo(memoryStream);
                        model.book.Photo = memoryStream.ToArray();
                        
                    }

                    // Save the new book to the database
                    await _books.CreateBook(model.book);

                    // Store a success message in TempData to show to the user
                    TempData["SuccessMessage"] = $"Book \"{model.book.Name}\" was added successfully";

                    // Return the same view (you could also redirect if desired)
                    return View();
                }
                catch (Exception ex)
                {
                    // If there's an error, store an error message in TempData
                    TempData["UnSuccessMessage"] = $"Book \"{ex.Message}\" was added successfully";
                }
            }
            else
            {
                TempData["UnSuccessMessage"] = $"Please enter a valid format for book";
            }


            // If model is not valid or there was an error, return the view with the model so user can correct input
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Borrow()
        {


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> DeleteBook(long inventoryNumber, string deleteReason)
        {
            var book = new BookModel();

            book.Id = inventoryNumber;

            await _books.DeleteBook(book, deleteReason);

            return RedirectToAction("Index");
        }
    }
}
