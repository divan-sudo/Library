using Library.Interfaces;

namespace Library.Services
{
    public class EmailService : IEmailService
    {
        private readonly List<EmailMessage> _sentEmails = new List<EmailMessage>();
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new EmailMessage
            {
                To = to,
                Subject = subject,
                Body = body,
                SentAt = DateTime.Now
            };
            _sentEmails.Add(email);
            _logger.LogInformation($"Email sent to {to}. Subject: {subject}");
            return Task.CompletedTask;
        }

        public IReadOnlyList<EmailMessage> GetSentEmails()
        {
            return _sentEmails.AsReadOnly();
        }

        public class EmailMessage
        {
            public string? To { get; set; }
            public string? Subject { get; set; }
            public string? Body { get; set; }
            public DateTime SentAt { get; set; }
        }
    }
}
