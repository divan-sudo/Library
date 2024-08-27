using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Book> Books { get; set; } = default!;
        public DbSet<Loan> Loans { get; set; } = default!;
    }
}
