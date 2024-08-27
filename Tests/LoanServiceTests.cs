using Library.Data;
using Library.Models;
using Library.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.Tests
{
    public class LoanServiceTests
    {
        private readonly DbContextOptions<LibraryContext> _options;
        private readonly DateTime _today = DateTime.Today;

        public LoanServiceTests()
        {
            _options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "TestLibraryDatabase")
                .Options;

            using (var context = new LibraryContext(_options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var tomorrow = _today.AddDays(1);

                context.Loans.AddRange(
                    new Loan
                    {
                        Id = 1,
                        DueDate = _today.AddDays(-1), // Overdue
                        LoanDate = _today.AddDays(-14),
                        ReturnDate = null,
                        Book = new Book { Id = 1, Title = "Book 1", Author = "Author 1", ISBN = "012345678901" },
                        User = new User { Id = 1, Name = "User 1", Email = "user1@example.com" }
                    },
                    new Loan
                    {
                        Id = 2,
                        DueDate = tomorrow, // Due tomorrow
                        LoanDate = _today.AddDays(-13),
                        ReturnDate = null,
                        Book = new Book { Id = 2, Title = "Book 2", Author = "Author 2", ISBN = "012345678902" },
                        UserId = 1
                    },
                    new Loan
                    {
                        Id = 3,
                        DueDate = tomorrow, // Also due tomorrow
                        LoanDate = _today.AddDays(-14),
                        ReturnDate = null,
                        Book = new Book { Id = 3, Title = "Book 3", Author = "Author 3", ISBN = "012345678903" },
                        User = new User { Id = 2, Name = "User 2", Email = "user2@example.com" }
                    },
                    new Loan
                    {
                        Id = 4,
                        DueDate = _today.AddDays(-2), // Overdue but returned
                        LoanDate = _today.AddDays(-16),
                        ReturnDate = _today.AddDays(-1),
                        Book = new Book { Id = 4, Title = "Book 4", Author = "Author 4", ISBN = "012345678904" },
                        User = new User { Id = 3, Name = "User 3", Email = "user3@example.com" }
                    },
                    new Loan
                    {
                        Id = 5,
                        DueDate = _today.AddDays(2), // Due in 2 days
                        LoanDate = _today.AddDays(-12),
                        ReturnDate = null,
                        Book = new Book { Id = 5, Title = "Book 5", Author = "Author 5", ISBN = "012345678905" },
                        User = new User { Id = 4, Name = "User 4", Email = "user4@example.com" }
                    },
                    new Loan
                    {
                        Id = 6,
                        DueDate = _today.AddDays(2), // Returned
                        LoanDate = _today.AddDays(-12),
                        ReturnDate = _today.AddDays(-1),
                        Book = new Book { Id = 6, Title = "Book 6", Author = "Author 6", ISBN = "012345678906" },
                        User = new User { Id = 5, Name = "User 5", Email = "user5@example.com" }
                    }
                );

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetAllLones_ShouldReturnCorrectLoans()
        {
            using var context = new LibraryContext(_options);
            var loanService = new LoanService(context);

            var allLoans = await loanService.GetAllLoansAsync();

            Assert.Equal(6, allLoans.Count());
            Assert.Contains(allLoans, l => l.Id == 1);
            Assert.Contains(allLoans, l => l.Id == 2);
            Assert.Contains(allLoans, l => l.Id == 3);
            Assert.Contains(allLoans, l => l.Id == 4);
            Assert.Contains(allLoans, l => l.Id == 5);
            Assert.Contains(allLoans, l => l.Id == 6);
        }

        [Fact]
        public async Task GetOverdueLoans_ShouldReturnCorrectLoans()
        {
            using var context = new LibraryContext(_options);
            var loanService = new LoanService(context);

            var overdueLoans = await loanService.GetOverdueLoansAsync();

            Assert.Single(overdueLoans);
            Assert.Contains(overdueLoans, l => l.Id == 1);
        }

        [Fact]
        public async Task GetLoansDueTomorrow_ShouldReturnCorrectLoans()
        {
            using var context = new LibraryContext(_options);
            var loanService = new LoanService(context);

            var loansDueTomorrow = await loanService.GetLoansDueTomorrowAsync();

            Assert.Equal(2, loansDueTomorrow.Count());
            Assert.Contains(loansDueTomorrow, l => l.Id == 2);
            Assert.Contains(loansDueTomorrow, l => l.Id == 3);
        }

        [Fact]
        public async Task GetReturnedLones_ShouldReturnCorrectLoans()
        {
            using var context = new LibraryContext(_options);
            var loanService = new LoanService(context);

            var allLoans = await loanService.GetAllLoansAsync();
            var returnedLoans = allLoans.Where(l => l.ReturnDate != null).ToList();

            Assert.Equal(2, returnedLoans.Count());
            Assert.Contains(returnedLoans, l => l.Id == 4);
            Assert.Contains(returnedLoans, l => l.Id == 6);
        }

        [Fact]
        public async Task GetActiveLoans_ShouldReturnCorrectLoans()
        {
            using var context = new LibraryContext(_options);
            var loanService = new LoanService(context);

            var activeLoans = await loanService.GetActiveLoansAsync();

            Assert.Equal(4, activeLoans.Count());
            Assert.Contains(activeLoans, l => l.Id == 1);
            Assert.Contains(activeLoans, l => l.Id == 2);
            Assert.Contains(activeLoans, l => l.Id == 3);
            Assert.Contains(activeLoans, l => l.Id == 5);
        }

        [Fact]
        public async Task GetCorrectLoanById_ShouldReturnCorrectLoan()
        {
            using var context = new LibraryContext(_options);
            var loanService = new LoanService(context);

            var loan1 = await loanService.GetLoanByIdAsync(1);
            var loan2 = await loanService.GetLoanByIdAsync(2);

            Assert.Equal(1, loan1.Id);
            Assert.Equal(2, loan2.Id);
        }

        [Fact]
        public async Task GetCorrectLoanByUserId_ShouldReturnCorrectLoan()
        {
            using var context = new LibraryContext(_options);
            var loanService = new LoanService(context);

            var loansByUserId = await loanService.GetLoansByUserIdAsync(1);

            Assert.Equal(2, loansByUserId.Count());
        }

        [Fact]
        public async Task GetCorrectLoanByBookId_ShouldReturnCorrectLoan()
        {
            using var context = new LibraryContext(_options);
            var loanService = new LoanService(context);

            var loanByBookId = await loanService.GetLoansByBookIdAsync(1);

            Assert.Single(loanByBookId);
        }

        [Fact]
        public async Task CreateNewLoan_ShouldReturnCreatedLoan()
        {
            using var context = new LibraryContext(_options);
            var loanService = new LoanService(context);

            var allLoans = await loanService.GetAllLoansAsync();

            Assert.Equal(6, allLoans.Count());

            var newLoan = new Loan
            {
                Id = 9,
                DueDate = _today.AddDays(12),
                LoanDate = _today.AddDays(-2),
                ReturnDate = null,
                Book = new Book { Title = "Book 9", Author = "Author 9", ISBN = "012345678909" },
                User = new User { Name = "User 9", Email = "user9@example.com" }
            };

            await loanService.CreateLoanAsync(newLoan);

            var allLoansNew = await loanService.GetAllLoansAsync();

            Assert.Equal(7, allLoansNew.Count());
        }

        [Fact]
        public async Task UpdateExistingLoan_ShouldReturnUpdatedLoan()
        {
            using var context = new LibraryContext(_options);
            var loanService = new LoanService(context);

            var loanToUpdate = await loanService.GetLoanByIdAsync(3);

            Assert.Equal(_today.AddDays(1), loanToUpdate.DueDate);
            
            loanToUpdate.DueDate = _today.AddDays(5);

            await loanService.UpdateLoanAsync(loanToUpdate);

            var updatedLoan = await loanService.GetLoanByIdAsync(3);

            Assert.Equal(_today.AddDays(5), loanToUpdate.DueDate);
        }

        [Fact]
        public async Task DeleteExistingLoan_ShouldDeleteLoan()
        {
            using var context = new LibraryContext(_options);
            var loanService = new LoanService(context);

            await loanService.DeleteLoanAsync(3);

            var allLoans = await loanService.GetAllLoansAsync();

            Assert.Equal(5, allLoans.Count());
        }
    }
}