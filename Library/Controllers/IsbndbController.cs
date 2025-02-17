using Library.Services.Isbndb;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers
{
    public class IsbndbController : Controller
    {
        private readonly IIsbndbService _isbn;
        public IsbndbController(IIsbndbService isbn)
        {
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
                await _isbn.GetBook(isbn);  
            }
               
            return View(isbn);

        }
    }
}
