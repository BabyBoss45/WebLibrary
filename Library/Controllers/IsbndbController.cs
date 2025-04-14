using Library.Services.Books;
using Library.Services.Books.Models;
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

        [HttpPost]// getting a user input of code, make api call for book and then save it 
        public async Task<IActionResult> GetBook([StringLength(13, MinimumLength = 10)] string isbn, string LibraryName)
        {
            // Check if the provided ISBN meets the validation rules
            if (ModelState.IsValid)
            {
                try
                {
                    // Call a service to fetch book details using the provided ISBN
                    var book = await _isbn.GetBook(isbn);

                    //passing the library name
                    book.LibName = LibraryName;

                    //checking if book is not null to save it in database
                    if (book != null)
                    {
                        // Save the retrieved book into the dataBase
                        await _books.CreateBook(book);
                        //sending message that book was added successfully 
                        TempData["SuccessMessage"] = $"Book \"{book.Name}\" was added successfully";
                    }
                    else
                    {
                        //sending message that book was not added successfully 
                        TempData["UnsuccessMessage"] = $"Book with this \"{isbn}\" was not found";
                    }


                }
                //cathcing errors if there was problems of adding book or it was not found
                catch (Exception ex)
                {
                    TempData["UnsuccessMessage"] = $"Book with this \"{isbn}\" was not found: {ex.Message}";
                }
            }
            else
            {
                //sendinng message that the syntax of input is not valid
                TempData["UnsuccessMessage"] = $"Invalid Format of Isbn, have to be 10 or 13 numbers";
            }

            return View("Views/Books/BooksManager.cshtml");

        }
    }
}
