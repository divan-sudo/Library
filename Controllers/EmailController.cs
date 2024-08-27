using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult SentEmails()
        {
            var sentEmails = _emailService.GetSentEmails();
            return View(sentEmails);
        }
    }
}