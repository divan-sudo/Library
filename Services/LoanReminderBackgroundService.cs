using Library.Interfaces;

namespace Library.Services
{
    public class LoanReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LoanReminderBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
        // faking this to represent 24h, so that it runs "daily"

        public LoanReminderBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<LoanReminderBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Loan reminder service is checking for overdue loans.");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var loanReminderService = scope.ServiceProvider.GetRequiredService<ILoanReminderService>();

                    await loanReminderService.SendAutomatedDueReminders();
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}