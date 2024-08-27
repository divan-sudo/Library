using static Library.Services.EmailService;

namespace Library.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        IReadOnlyList<EmailMessage> GetSentEmails();
    }
}