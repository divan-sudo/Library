using Library.Data;
using Library.Models;
using Library.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.Tests
{
    public class BookServiceTests
    {
        private readonly DbContextOptions<LibraryContext> _options;
        private readonly DateTime _today = DateTime.Today;

        public BookServiceTests()
        {
            _options = new DbContextOptionsBuilder<LibraryContext>()
               .UseInMemoryDatabase(databaseName: "TestLibraryDatabase")
               .Options;

            using (var context = new LibraryContext(_options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Books.AddRange(
                    new Book
                    {
                        Id = 1,
                        Title = "Title 1",
                        Author = "Author 1",
                        ISBN = "012345678901",
                        CreatedDate = _today,
                    },
                    new Book
                    {
                        Id = 2,
                        Title = "Title 2",
                        Author = "Author 2",
                        ISBN = "012345678902",
                        CreatedDate = _today.AddDays(-1),
                    }
                );

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnAllBooks()
        {
            using var context = new LibraryContext(_options);
            var bookService = new BookService(context);

            var allBooks = await bookService.GetAllBooksAsync();

            Assert.Equal(2, allBooks.Count());
            Assert.Contains(allBooks, b => b.Title == "Title 1");
            Assert.Contains(allBooks, b => b.Title == "Title 2");
        }

        [Fact]
        public async Task GetBookById_ShouldReturnCorrectBook()
        {
            using var context = new LibraryContext(_options);
            var bookService = new BookService(context);

            var book = await bookService.GetBookByIdAsync(2);

            Assert.Equal("Title 2", book.Title);
        }

        [Fact]
        public async Task UpdateExistingBook_ShouldReturnUpdatedBook()
        {
            using var context = new LibraryContext(_options);
            var bookService = new BookService(context);

            var bookToUpdate = await bookService.GetBookByIdAsync(1);

            Assert.Equal("Title 1", bookToUpdate.Title);

            bookToUpdate.Title = "New Title 1";

            await bookService.UpdateBookAsync(bookToUpdate);

            var updatedBook = await bookService.GetBookByIdAsync(1);

            Assert.Equal("New Title 1", updatedBook.Title);
        }

        [Fact]
        public async Task DeleteExistingBook_ShouldDeleteBook()
        {
            using var context = new LibraryContext(_options);
            var bookService = new BookService(context);

            await bookService.DeleteBookAsync(2);

            var allBooks = await bookService.GetAllBooksAsync();

            Assert.Single(allBooks);
        }
    }
}
