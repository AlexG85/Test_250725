# Test_250725
Development of a Practical test

## Overview

Test_Examen is a .NET 8 web API solution designed as a practical exam project. It demonstrates modern ASP.NET Core development practices, including JWT authentication, custom middleware, user management, and reporting features.

## Projects

- **Test_Examen**: Main ASP.NET Core Web API project.
- **Testing**: xUnit test project for controller and service logic.

## Features

- **JWT Authentication**: Secure endpoints using JSON Web Tokens.
- **Custom Middleware**: Handles token validation and blacklisting.
- **User Management**: Endpoints for login, registration, token refresh, and user session reporting.
- **Role Management**: CRUD endpoints for application roles.
- **Employee Management**: CRUD endpoints for employee records.
- **Swagger/OpenAPI**: Interactive API documentation.
- **Entity Framework Core**: SQL Server database integration with automatic migrations.
- **Authorization**: Role-based and policy-based access control.
- **Unit Testing**: xUnit and Moq for controller and service tests.
- **In-memory Caching**: Used for token blacklisting and performance.
- **Hosted Services**: Background service for invalid token management.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local or cloud)
- Visual Studio 2022 or later

### Configuration

Update `appsettings.json` with your database connection string and JWT settings:
"ConnectionStrings": { "DefaultConnection": "Server=...;Database=...;User Id=...;Password=...;" }, "Authentication": { "Jwt": { "Issuer": "your-issuer", "Audience": "your-audience", "SigningKey": "your-signing-key", "ExpireMinutes": 60 } }


### Build and Run

1. Restore NuGet packages: dotnet restore

2. Build the solution: dotnet build
   
3. Apply database migrations (optional, runs automatically on startup): dotnet ef datatabase update

4. Run the API: dotnet run --project Test_Examen

5. Access Swagger UI at [http://localhost:5000/swagger](http://localhost:5000/swagger) (default port may vary).

6. You can also access to the Azure Demo: https://testexamenapp.azurewebsites.net/swagger/index.html

### Testing

Run unit tests with: dotnet test


## API Endpoints

### Authentication & Users

- `POST /Auth/Login` — Authenticate user and receive JWT.
- `POST /Auth/Signin` — Register a new user.
- `POST /Auth/Refresh` — Refresh JWT token.
- `GET /Auth/GetAuthenticated` — Get current authenticated user info.
- `GET /ReportUser/sessionlogins?userId={id}&size={n}` — Get recent login sessions for a user.

### Roles

- `GET /Role/all` — Get all roles.
- `GET /Role/{id}` — Get role by ID.
- `POST /Role` — Create a new role.
- `PUT /Role/{id}` — Update a role.
- `DELETE /Role/{id}` — Delete a role.

### Employees

- `GET /Employee/all` — Get all employees.
- `GET /Employee/{id}` — Get employee by ID.
- `POST /Employee` — Create a new employee.
- `PUT /Employee/{id}` — Update an employee.
- `DELETE /Employee/{id}` — Delete an employee.

## Technologies Used

- ASP.NET Core 8
- Entity Framework Core
- JWT Bearer Authentication
- xUnit & Moq (for testing)
- Swagger (Swashbuckle)
- In-memory caching
- Hosted background services

## License

This project is for testing skills and demonstration purposes for a new job application.


