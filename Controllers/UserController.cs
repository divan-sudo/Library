using Library.Interfaces;
using Library.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering UserController.Index");
            try
            {
                var users = await _userService.GetAllUsersAsync();
                _logger.LogInformation($"Retrieved {users.Count()} books");

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in UserController.Index");

                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Entering UserController.Details with id {id}");
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with id {id} not found");

                    return NotFound();
                }

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UserController.Details for id {id}");

                return View("Error");
            }
        }

        public IActionResult Create()
        {
            _logger.LogInformation("Entering UserController.Create GET");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in UserController.Create GET");

                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email")] User user)
        {
            _logger.LogInformation("Entering UserController.Create POST");
            try
            {
                if (ModelState.IsValid)
                {
                    await _userService.CreateUserAsync(user);
                    _logger.LogInformation($"New user created, with id: {user.Id}");

                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "New book created successfully.";

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in UserController.Create POST");

                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation($"Entering UserController.Edit GET with id {id}");
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with id {id} not found");

                    return NotFound();
                }

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UserController.Edit GET for id {id}");

                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Email")] User user)
        {
            _logger.LogInformation($"Entering UserController.Edit POST with id {id}");
            try
            {
                if (ModelState.IsValid)
                {
                    user.UpdatedDate = DateTime.Now;
                    await _userService.UpdateUserAsync(user);
                    _logger.LogInformation($"Updated user with id: {user.Id}");
                    TempData["SuccessMessage"] = "User updated successfully.";

                    return RedirectToAction(nameof(Index));
                }
                
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UserController.Edit POST for id {id}");

                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Entering UserController.Delete GET with id: {id}");
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with id: {id}, not found");

                    return NotFound();
                }

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UserController.Delete GET for id: {id}");

                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation($"Entering UserController.DeleteConfirmed with id: {id}");
            try
            {
                await _userService.DeleteUserAsync(id);
                _logger.LogInformation($"User with id: {id}, deleted");
                TempData["SuccessMessage"] = "User deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UserController.DeleteConfirmed for id: {id}");

                return View("Error");
            }
        }
    }
}
