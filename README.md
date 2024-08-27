# Library Management System

## Project Overview
This is a sample library management system that allows for the management of books, users, and loans. It includes features such as CRUD operations for books, users, and loans, as well as an automated notification system for overdue books.

## Technologies Used
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server (LocalDB)
- C#

## Setup Instructions

### Prerequisites
- Visual Studio 2022
- .NET 6.0 SDK or later
- SQL Server LocalDB

### Getting Started
1. Clone the repository to your local machine.
2. Open the solution in Visual Studio 2022.
3. Open the Package Manager Console in VS2022.
4. Create the initial migration:
   ```
   add-migration InitialCreate
   ```
5. Update the database:
   ```
   Update-Database
   ```
6. Run the application.

### Database Seeding
- Database seeding occurs automatically on startup if the database is empty.
- It creates:
  - A few sample books
  - A few sample users
  - A few sample loans (in different states)

## Features
- CRUD operations for Books, Users, and Loans
- Automated notification system for overdue books
- Manual notification sending for specific loans
- Fake email service to simulate sending notification emails
- Simple listing page to display "sent emails"

## Usage
- Feel free to create/add new books, users, or loans using the provided interfaces.
- The application includes full CRUD (Create, Read, Update, Delete) operations for all models.

## Background Services
- The app includes a background service that runs at 1-minute intervals.
- This service mimics a 24-hour span for checking overdue books and sending automated notifications.

## Email Service
- The application uses a fake email service to simulate sending notification emails.
- Emails can be sent both automatically (for overdue books) and manually (for specific loans).
- A simple listing page is available to display all "sent emails".

## Testing
The project includes both unit tests and integration tests to ensure code quality and functionality:

- **BookServiceTests**: Unit tests for the Book Service, covering CRUD operations and business logic related to books.
- **LoanServiceTests**: Unit tests for the Loan Service, ensuring correct handling of loan operations, due dates, and overdue calculations.
- **LoanIntegrationTests**: Integration tests for the Loan functionality, testing the entire stack from HTTP requests through to database operations.

To run the tests:
1. Open the solution in Visual Studio 2022.
2. Open the Test Explorer (Test > Test Explorer).
3. Click "Run All Tests" in the Test Explorer or run individual tests.

## Future Enhancements

### Docker Integration
The next major enhancement for this project is Docker integration. This will involve:

1. Creating a Dockerfile for the application.
2. Setting up docker-compose for easy deployment of the app and its dependencies (e.g., SQL Server).
3. Configuring environment-specific settings for development, testing, and production environments.

### Other Potential Improvements
- **CI/CD Pipeline**: Implement a continuous integration and deployment pipeline using GitHub Actions or Azure DevOps.
- **API Documentation**: Add Swagger/OpenAPI documentation for better API discoverability and testing.
- **Performance Optimization**: Implement caching mechanisms and optimize database queries for improved performance.
- **User Authentication and Authorization**: Implement a robust auth system, possibly using Identity Server or Azure AD.
- **Expanded Test Coverage**: Add more integration tests and implement end-to-end (E2E) tests using a tool like Selenium or Cypress.
- **Localization**: Add support for multiple languages to make the application accessible to a wider audience.
- **Search Functionality**: Implement full-text search for books using Elasticsearch or Azure Search.
- **Reporting**: Add a reporting module for generating insights about library usage, popular books, etc.
- **Include external API**: Api to [https://isbndb.com](https://isbndb.com/apidocs/v2) to get book details by ISBN like (Image, Author, Title, Publisher and etc.)
## Contributing
Contributions to the project are welcome. If you're interested in adding any of the above enhancements or have other ideas, please feel free to fork the repository and submit a pull request.

