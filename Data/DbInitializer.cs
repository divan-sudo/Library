using Library.Models;

namespace Library.Data
{
    public class DbInitializer
    {
        public static void Initialize(LibraryContext context)
        {
            context.Database.EnsureCreated();

            if (context.Books.Any())
            {
                return;
            }

            var books = new Book[]
            {
                new Book { 
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ISBN = "9780743273565",
                    CreatedDate = DateTime.Now
                },
                new Book { 
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    ISBN = "9780446310789",
                    CreatedDate = DateTime.Now
                },
                new Book { 
                    Title = "Surrounded by Idiots",
                    Author = "Thomas Erikson",
                    ISBN = "9781785042188",
                    CreatedDate = DateTime.Now
                },
                new Book { 
                    Title = "Wit",
                    Author = "Des MacHale",
                    ISBN = "9780740733307",
                    CreatedDate = DateTime.Now
                },
                new Book { Title = "All My Friends Are Dead",
                    Author = "Avery Monsen | Jory John",
                    ISBN = "9780811874557",
                    CreatedDate = DateTime.Now
                },
                new Book {
                    Title = "The Book of Useless Information",
                    Author = "Noel Botham",
                    ISBN = "9780399532696",
                    CreatedDate = DateTime.Now
                },
                new Book {
                    Title = "The Psychology of Stupidity",
                    Author = "Jean-Francois Marmion",
                    ISBN = "9781529053869", 
                    CreatedDate = DateTime.Now
                }
            };

            foreach (Book b in books)
            {
                context.Books.Add(b);
            }
            context.SaveChanges();

            var users = new User[]
            {
                new User {
                    Name = "Sheldon Cooper",
                    Email = "sheldon.lee.cooper@example.com",
                    CreatedDate = DateTime.Now
                },
                new User {
                    Name = "SpongeBob SquarePants ",
                    Email = "bob.square@example.com",
                    CreatedDate = DateTime.Now
                },
                new User {
                    Name = "Carlos Irwin Estévez",
                    Email = "charlie.sheen@example.com",
                    CreatedDate = DateTime.Now
                },
                new User {
                    Name = "Billy Bob Thornton",
                    Email = "billy.bob@example.com",
                    CreatedDate = DateTime.Now
                }
            };

            foreach (User u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();

            var loans = new Loan[]
            {
                new Loan {
                    UserId = 1,
                    BookId = 1,
                    LoanDate = DateTime.Now.AddDays(-2),
                    DueDate = DateTime.Now.AddDays(12),
                    CreatedDate = DateTime.Now.AddDays(-2)
                },
                new Loan {
                    UserId = 2,
                    BookId = 2,
                    LoanDate = DateTime.Now.AddDays(-7),
                    DueDate = DateTime.Now.AddDays(7),
                    CreatedDate = DateTime.Now.AddDays(-7)
                },
                new Loan {
                    UserId = 3,
                    BookId = 3,
                    LoanDate = DateTime.Now.AddDays(-12),
                    DueDate = DateTime.Now.AddDays(2),
                    CreatedDate = DateTime.Now.AddDays(-12)
                },
                new Loan {
                    UserId = 3,
                    BookId = 5,
                    LoanDate = DateTime.Now.AddDays(-13),
                    DueDate = DateTime.Now.AddDays(1),
                    CreatedDate = DateTime.Now.AddDays(-13)
                },
                new Loan {
                    UserId = 4,
                    BookId = 4,
                    LoanDate = DateTime.Now.AddDays(-17),
                    DueDate = DateTime.Now.AddDays(-3),
                    ReturnDate = DateTime.Now.AddDays(-2),
                    CreatedDate = DateTime.Now.AddDays(-17),
                    UpdatedDate = DateTime.Now.AddDays(-2)
                },
                new Loan {
                    UserId = 1,
                    BookId = 6,
                    LoanDate = DateTime.Now.AddDays(-18),
                    DueDate = DateTime.Now.AddDays(-4),
                    CreatedDate = DateTime.Now.AddDays(-18)
                },
                new Loan {
                    UserId = 2,
                    BookId = 7,
                    LoanDate = DateTime.Now.AddDays(-17),
                    DueDate = DateTime.Now.AddDays(-3),
                    CreatedDate = DateTime.Now.AddDays(-17)
                }
            };

            foreach (Loan l in loans)
            {
                context.Loans.Add(l);
            }
            context.SaveChanges();
        }
    }
}
