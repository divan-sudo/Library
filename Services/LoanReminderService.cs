using Library.Interfaces;

namespace Library.Services
{
    public class LoanReminderService : ILoanReminderService
    {
        private readonly IEmailService _emailService;
        private readonly ILoanService _loanService;

        public LoanReminderService(IEmailService emailService, ILoanService loanService)
        {
            _emailService = emailService;
            _loanService = loanService;
        }

        public async Task SendAutomatedDueReminders()
        {
            var dueTomorrowLoans = await _loanService.GetLoansDueTomorrowAsync();

            foreach (var loan in dueTomorrowLoans)
            {
                await _emailService.SendEmailAsync(
                    loan.User.Email,
                    "Automatic Reminder: Book Due Tomorrow",
                    $"Dear {loan.User.Name},\n\nThis is a reminder that your book '{loan.Book.Title}' is due tomorrow. Please return it to avoid late fees.\n\nThank you,\nYour Library"
                );
            }
        }
    }
}