using Library.Data;
using Library.Interfaces;
using Library.Models;
using Library.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using Xunit;

namespace Library.Tests.Integration
{
    public class LoanIntegrationTests : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        public LoanIntegrationTests()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.AddDbContext<LibraryContext>(options =>
                        options.UseInMemoryDatabase("TestLibraryDatabase"));

                    services.AddScoped<ILoanService, LoanService>();
                    services.AddScoped<IBookService, BookService>();
                    services.AddScoped<IUserService, UserService>();
                    services.AddScoped<IEmailService, EmailService>();

                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.AddConsole();
                        loggingBuilder.AddDebug();
                    });

                    services.AddControllersWithViews();
                    services.AddAntiforgery(options =>
                    {
                        options.HeaderName = "X-CSRF-TOKEN";
                        options.Cookie.Name = "XSRF-TOKEN";
                    });
                })
                .Configure(app =>
                {
                    app.UseDeveloperExceptionPage();
                    app.UseRouting();
                    app.UseAntiforgery();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllerRoute(
                            name: "default",
                            pattern: "{controller=Home}/{action=Index}/{id?}");
                    });
                });

            _server = new TestServer(builder);
            _client = _server.CreateClient();
            _serviceProvider = _server.Services;

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();
                SeedDatabase(context);
            }
        }

        private void SeedDatabase(LibraryContext context)
        {
            var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "1234567890", CreatedDate = DateTime.Now.AddDays(-21) };
            context.Books.Add(book);

            var user = new User { Id = 1, Name = "Test User", Email = "test@example.com", CreatedDate = DateTime.Now.AddDays(-21) };
            context.Users.Add(user);

            var overdueLoan = new Loan
            {
                Id = 1,
                BookId = 1,
                UserId = 1,
                LoanDate = DateTime.Today.AddDays(-21),
                DueDate = DateTime.Today.AddDays(-7),
                CreatedDate = DateTime.Now.AddDays(-21),
                Book = book,
                User = user
            };
            context.Loans.Add(overdueLoan);

            context.SaveChanges();
        }

        [Fact]
        public async Task SendOverdueNotification_ShouldSendNotificationAndRedirectToDetails()
        {
            var antiforgery = _serviceProvider.GetRequiredService<IAntiforgery>();
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var tokens = antiforgery.GetAndStoreTokens(httpContext);

            var content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");
            _client.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={tokens.CookieToken}");
            _client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", tokens.RequestToken);

            var response = await _client.PostAsync("/Loan/SendOverdueNotification/1", content);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("/Loan/Details/", response.Headers.Location?.ToString());

            var emailService = _serviceProvider.GetRequiredService<IEmailService>();
            Assert.True(emailService.GetSentEmails() != null);
        }

        [Fact]
        public async Task ReturnLoan_ShouldUpdateLoanAndRedirectToIndex()
        {
            var antiforgery = _serviceProvider.GetRequiredService<IAntiforgery>();
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var tokens = antiforgery.GetAndStoreTokens(httpContext);
            var dbContext = _serviceProvider.GetRequiredService<LibraryContext>();

            var content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");
            _client.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={tokens.CookieToken}");
            _client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", tokens.RequestToken);

            var response = await _client.PostAsync("/Loan/Return/1", content);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("/Loan", response.Headers.Location?.ToString());

            var returnedLoan = dbContext.Loans.Where(l => l.Id == 1).FirstOrDefault();
            Assert.True(returnedLoan.ReturnDate != null);
        }

        [Fact]
        public async Task CreateLoan_ShouldCreateLoanAndRedirectToIndex()
        {
            var antiforgery = _serviceProvider.GetRequiredService<IAntiforgery>();
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var tokens = antiforgery.GetAndStoreTokens(httpContext);
            var dbContext = _serviceProvider.GetRequiredService<LibraryContext>();

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Id", "2"),
                new KeyValuePair<string, string>("BookId", "1"),
                new KeyValuePair<string, string>("UserId", "1"),
                new KeyValuePair<string, string>("LoanDate", DateTime.Today.ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("DueDate", DateTime.Today.AddDays(14).ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("CreatedDate", DateTime.Today.ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("__RequestVerificationToken", tokens.RequestToken)
            });

            _client.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={tokens.CookieToken}");
            _client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", tokens.RequestToken);

            var response = await _client.PostAsync("/Loan/Create", formContent);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Loan", response.Headers.Location?.ToString());

            var allLoans = dbContext.Loans.ToList();
            Assert.Contains(allLoans, l => l.Id == 2);
        }

        [Fact]
        public async Task EditLoan_ShouldEditLoanAndRedirectToIndex()
        {
            var antiforgery = _serviceProvider.GetRequiredService<IAntiforgery>();
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var tokens = antiforgery.GetAndStoreTokens(httpContext);
            var dbContext = _serviceProvider.GetRequiredService<LibraryContext>();

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("DueDate", DateTime.Today.AddDays(17).ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("UpdatedDate", DateTime.Now.ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("__RequestVerificationToken", tokens.RequestToken)
            });

            _client.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={tokens.CookieToken}");
            _client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", tokens.RequestToken);

            var response = await _client.PostAsync("/Loan/Edit/1", formContent);
            
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Loan", response.Headers.Location?.ToString());

            var edditedLoan = dbContext.Loans.Where(l => l.Id == 1).FirstOrDefault();
            Assert.Equal(DateTime.Now.Date, edditedLoan.UpdatedDate.Value.Date);
        }

        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}