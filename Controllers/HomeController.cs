using Library.Interfaces;
using Library.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoanService _loanService;

        public HomeController(ILoanService loanService, ILogger<HomeController> logger)
        {
            _loanService = loanService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            //var recentLoans = await _loanService.GetRecentLoansAsync(5);
            //var overdueLoans = await _loanService.GetOverdueLoansAsync();

            //var viewModel = new HomeViewModel
            //{
            //    RecentLoans = recentLoans,
            //    OverdueLoans = overdueLoans
            //};

            return View(
                //viewModel
            );
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
