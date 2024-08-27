using Library.Data;
using Library.Interfaces;
using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Services
{
    public class LoanService : ILoanService
    {
        private readonly LibraryContext _context;
        private readonly DateTime _today = DateTime.Today;

        public LoanService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetLoansDueTomorrowAsync()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.DueDate >= _today.AddDays(1) && l.DueDate < _today.AddDays(2) && l.ReturnDate == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.DueDate < _today && l.ReturnDate == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
        {
            return await _context.Loans
                .Where(l => l.ReturnDate == null)
                .Include(l => l.User)
                .Include(l => l.Book)
                .ToListAsync();
        }

        public async Task<Loan> GetLoanByIdAsync(int id)
        {
            return await _context.Loans
                .Include(l => l.User)
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Loan>> GetLoansByUserIdAsync(int userId)
        {
            return await _context.Loans
                .Where(l => l.UserId == userId)
                .Include(l => l.Book)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetLoansByBookIdAsync(int bookId)
        {
            return await _context.Loans
                .Where(l => l.BookId == bookId)
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<Loan> CreateLoanAsync(Loan loan)
        {
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return loan;
        }

        public async Task UpdateLoanAsync(Loan loan)
        {
            _context.Entry(loan).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLoanAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null)
            {
                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();
            }
        }
    }
}
