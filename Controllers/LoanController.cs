using Library.Interfaces;
using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ILogger<LoanController> _logger;

        public LoanController(ILoanService loanService, IBookService bookService, IUserService userService, IEmailService emailService, ILogger<LoanController> logger)
        {
            _loanService = loanService;
            _bookService = bookService;
            _userService = userService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering LoanController.Index");
            try
            {
                var loans = await _loanService.GetAllLoansAsync();
                _logger.LogInformation($"Retrieved {loans.Count()} loans");

                return View(loans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in LoanController.Index");

                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Entering LoanController.Details with id: {id}");
            try
            {
                var loan = await _loanService.GetLoanByIdAsync(id);
                if (loan == null)
                {
                    _logger.LogWarning($"Loan with id: {id} not found");

                    return NotFound();
                }

                return View(loan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in LoanController.Details for id: {id}");

                return View("Error");
            }
        }

        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Entering LoanController.Create GET");
            try
            {
                ViewData["BookId"] = new SelectList(await _bookService.GetAllBooksAsync(), "Id", "Title");
                ViewData["UserId"] = new SelectList(await _userService.GetAllUsersAsync(), "Id", "Name");

                var loan = new Loan
                {
                    LoanDate = DateTime.Today,
                    DueDate = DateTime.Today.AddDays(14)
                };

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in LoanController.Create GET");

                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,UserId,LoanDate,DueDate")] Loan loan)
        {
            _logger.LogInformation("Entering LoanController.Create POST");
            try
            {
                if (ModelState.IsValid)
                {
                    await _loanService.CreateLoanAsync(loan);
                    _logger.LogInformation($"Created new loan with id: {loan.Id}");
                    TempData["SuccessMessage"] = "New loan created successfully.";

                    return RedirectToAction(nameof(Index));
                }
                 
                ViewData["BookId"] = new SelectList(await _bookService.GetAllBooksAsync(), "Id", "Title", loan.BookId);
                ViewData["UserId"] = new SelectList(await _userService.GetAllUsersAsync(), "Id", "Name", loan.UserId);
                
                return View(loan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in LoanController.Create POST");

                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation($"Entering LoanController.Edit GET with id: {id}");
            try
            {
                var loan = await _loanService.GetLoanByIdAsync(id);
                if (loan == null)
                {
                    _logger.LogWarning($"Loan with id: {id} not found");

                    return NotFound();
                }

                if (loan.ReturnDate != null)
                {
                    _logger.LogWarning($"Loan with id: {id} is already returned");
                    ModelState.AddModelError("Returned", "Returned loans cannot be edited.");
                    TempData["ErrorMessage"] = "Returned loans cannot be edited.";

                    return View(loan);
                }

                return View(loan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in LoanController.Edit GET for id: {id}");

                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,UserId,LoanDate,DueDate,ReturnDate")] Loan loan)
        {
            _logger.LogInformation($"Entering LoanController.Edit POST with id: {id}");
            try
            {
                if (ModelState.IsValid)
                {
                    var existingLoan = await _loanService.GetLoanByIdAsync(id);
                    if (existingLoan == null)
                    {
                        return NotFound();
                    }

                    if (existingLoan.LoanDate == loan.LoanDate &&
                        existingLoan.DueDate == loan.DueDate &&
                        existingLoan.ReturnDate == loan.ReturnDate &&
                        true)
                    {
                        TempData["Message"] = "No changes were made.";

                        return RedirectToAction(nameof(Index));
                    }

                    existingLoan.LoanDate = loan.LoanDate;
                    existingLoan.DueDate = loan.DueDate;
                    existingLoan.ReturnDate = loan.ReturnDate;
                    existingLoan.UpdatedDate = DateTime.Now;

                    await _loanService.UpdateLoanAsync(existingLoan);
                    _logger.LogInformation($"Loan with id: {loan.Id} updated");
                    TempData["SuccessMessage"] = "Loan updated successfully.";

                    return RedirectToAction(nameof(Index));
                }
                
                return View(loan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in LoanController.Edit POST for id: {id}");

                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Entering LoanController.Delete GET with id: {id}");
            try
            {
                var loan = await _loanService.GetLoanByIdAsync(id);
                if (loan == null)
                {
                    _logger.LogWarning($"Loan with id: {id} not found");

                    return NotFound();
                }

                return View(loan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in LoanController.Delete GET for id: {id}");

                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation($"Entering LoanController.DeleteConfirmed with id: {id}");
            try
            {
                await _loanService.DeleteLoanAsync(id);
                _logger.LogInformation($"Loan with id: {id} deleted");
                TempData["SuccessMessage"] = "Loan deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in LoanController.DeleteConfirmed for id: {id}");

                return View("Error");
            }
        }

        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id)
        {
            _logger.LogInformation($"Entering LoanController.Return with id: {id}");
            try
            {
                var loan = await _loanService.GetLoanByIdAsync(id);
                if (loan == null)
                {
                    _logger.LogWarning($"Loan with id: {id} not found");

                    return NotFound();
                }

                if (loan.ReturnDate != null)
                {
                    TempData["ErrorMessage"] = "This loan has already been returned.";

                    return RedirectToAction(nameof(Index));
                }

                loan.ReturnDate = DateTime.Now;
                loan.UpdatedDate = DateTime.Now;
                await _loanService.UpdateLoanAsync(loan);

                TempData["SuccessMessage"] = "Book returned successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in LoanController.Return for id: {id}");

                return View("Error");
            }
        }
        
        [HttpPost, ActionName("SendOverdueNotification")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOverdueNotification(int id)
        {
            _logger.LogInformation($"Entering LoanController.SendOverdueNotification with id: {id}");
            try
            {
                var loan = await _loanService.GetLoanByIdAsync(id);
                if (loan == null)
                {
                    _logger.LogWarning($"Loan with id: {id} not found");

                    return NotFound();
                }

                if (loan.ReturnDate != null || loan.DueDate > DateTime.Now)
                {
                    TempData["ErrorMessage"] = "Cannot send overdue notification for non-overdue or returned loans.";

                    return RedirectToAction(nameof(Details), new { id = loan.Id });
                }

                var emailBody = $"Dear {loan.User.Name},\n\nThis is a reminder that your book '{loan.Book.Title}' is due tomorrow. Please return it to avoid late fees.\n\nThank you,\nYour Library";

                await _emailService.SendEmailAsync(loan.User.Email, "Manual Reminder: Book Due Tomorrow", emailBody);
                TempData["SuccessMessage"] = "Overdue notification sent successfully.";

                return RedirectToAction(nameof(Details), new { id = loan.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in LoanController.SendOverdueNotification for id: {id}");

                return View("Error");
            }
        }
    }
}