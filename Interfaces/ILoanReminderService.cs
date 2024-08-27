namespace Library.Interfaces
{
    public interface ILoanReminderService
    {
        Task SendAutomatedDueReminders();
    }
}