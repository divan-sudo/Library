using Library.Data;
using Library.Interfaces;
using Library.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoanReminderService, LoanReminderService>();

builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddHostedService<LoanReminderBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LibraryContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.MapControllers();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

#region BookRoutes
app.MapControllerRoute(
    name: "bookIndex",
    pattern: "Book/Index",
    defaults: new { controller = "Book", action = "Index" });
app.MapControllerRoute(
    name: "bookCreate",
    pattern: "Book/Create",
    defaults: new { controller = "Book", action = "Create" });
app.MapControllerRoute(
    name: "bookEdit",
    pattern: "Book/Edit/{id}",
    defaults: new { controller = "Book", action = "Edit" });
app.MapControllerRoute(
    name: "bookDetails",
    pattern: "Book/Details/{id}",
    defaults: new { controller = "Book", action = "Details" });
app.MapControllerRoute(
    name: "bookDelete",
    pattern: "Book/Delete/{id}",
    defaults: new { controller = "Book", action = "Delete" });
#endregion

#region LoanRoutes
app.MapControllerRoute(
    name: "loanIndex",
    pattern: "Loan/Index",
    defaults: new { controller = "Loan", action = "Index" });
app.MapControllerRoute(
    name: "loanCreate",
    pattern: "Loan/Create",
    defaults: new { controller = "Loan", action = "Create" });
app.MapControllerRoute(
    name: "loanEdit",
    pattern: "Loan/Edit/{id}",
    defaults: new { controller = "Loan", action = "Edit" });
app.MapControllerRoute(
    name: "loanDetails",
    pattern: "Loan/Details/{id}",
    defaults: new { controller = "Loan", action = "Details" });
app.MapControllerRoute(
    name: "loanDelete",
    pattern: "Loan/Delete/{id}",
    defaults: new { controller = "Loan", action = "Delete" });
#endregion

#region UserRoutes
app.MapControllerRoute(
    name: "userIndex",
    pattern: "User/Index",
    defaults: new { controller = "User", action = "Index" });
app.MapControllerRoute(
    name: "userCreate",
    pattern: "User/Create",
    defaults: new { controller = "User", action = "Create" });
app.MapControllerRoute(
    name: "userEdit",
    pattern: "User/Edit/{id}",
    defaults: new { controller = "User", action = "Edit" });
app.MapControllerRoute(
    name: "userDetails",
    pattern: "User/Details/{id}",
    defaults: new { controller = "User", action = "Details" });
app.MapControllerRoute(
    name: "userDelete",
    pattern: "User/Delete/{id}",
    defaults: new { controller = "User", action = "Delete" });
#endregion

// DefaultRoutes
app.MapControllerRoute(
    name: "sentEmails",
    pattern: "Email/SentEmails",
    defaults: new { controller = "Email", action = "SentEmails" });
app.MapControllerRoute(
    name: "home",
    pattern: "",
    defaults: new { controller = "Home", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}");


app.Run();
