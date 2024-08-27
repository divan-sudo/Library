using Library.Interfaces;
using Library.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering BookController.Index");
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                _logger.LogInformation($"Retrieved {books.Count()} books");

                return View(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in BookController.Index");

                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Entering BookController.Details with id {id}");
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                {
                    _logger.LogWarning($"Book with id {id} not found");

                    return NotFound();
                }

                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in BookController.Details for id {id}");

                return View("Error");
            }
        }

        public IActionResult Create()
        {
            _logger.LogInformation("Entering BookController.Create GET");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in BookController.Create GET");

                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,ISBN")] Book book)
        {
            _logger.LogInformation("Entering BookController.Create POST");
            try
            {
                if (ModelState.IsValid)
                {
                    await _bookService.CreateBookAsync(book);
                    _logger.LogInformation($"New book created, with id: {book.Id}");
                    TempData["SuccessMessage"] = "New book created successfully.";

                    return RedirectToAction(nameof(Index));
                }

                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in BookController.Create POST");

                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation($"Entering BookController.Edit GET with id :{id}");
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                {
                    _logger.LogWarning($"Book with id: {id}, not found");
                    return NotFound();
                }
                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in BookController.Edit GET for id: {id}");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,ISBN")] Book book)
        {
            _logger.LogInformation($"Entering BookController.Edit POST with id {id}");
            try
            {
                if (ModelState.IsValid)
                {
                    var existingBook = await _bookService.GetBookByIdAsync(id);
                    if (existingBook == null)
                    {
                        return NotFound();
                    }

                    if (existingBook.Title == book.Title &&
                        existingBook.Author == book.Author &&
                        existingBook.ISBN == book.ISBN &&
                        true)
                    {
                        TempData["Message"] = "No changes were made.";

                        return RedirectToAction(nameof(Index));
                    }

                    existingBook.Title = book.Title;
                    existingBook.Author = book.Author;
                    existingBook.ISBN = book.ISBN;
                    existingBook.UpdatedDate = DateTime.Now;

                    await _bookService.UpdateBookAsync(existingBook);
                    _logger.LogInformation($"Updated Book with id: {book.Id}");
                    TempData["SuccessMessage"] = "Book updated successfully.";

                    return RedirectToAction(nameof(Index));
                }

                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in BookController.Edit POST for id {id}");

                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Entering BookController.Delete GET with id: {id}");
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                {
                    _logger.LogWarning($"Book with id {id} not found");
                    return NotFound();
                }
                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in BookController.Delete GET for id: {id}");
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation($"Entering BookController.DeleteConfirmed with id: {id}");
            try
            {
                await _bookService.DeleteBookAsync(id);
                _logger.LogInformation($"Book with id: {id}, deleted");
                TempData["SuccessMessage"] = "Book deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in BookController.DeleteConfirmed for id: {id}");

                return View("Error");
            }
        }
    }
}