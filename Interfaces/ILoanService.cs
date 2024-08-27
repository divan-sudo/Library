using Library.Models;

namespace Library.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<Loan>> GetAllLoansAsync();
        Task<IEnumerable<Loan>> GetLoansDueTomorrowAsync();
        Task<IEnumerable<Loan>> GetActiveLoansAsync();
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();

        Task<Loan> GetLoanByIdAsync(int id);
        Task<IEnumerable<Loan>> GetLoansByUserIdAsync(int userId);
        Task<IEnumerable<Loan>> GetLoansByBookIdAsync(int bookId);
        Task<Loan> CreateLoanAsync(Loan loan);
        Task UpdateLoanAsync(Loan loan);
        Task DeleteLoanAsync(int id);
    }
}