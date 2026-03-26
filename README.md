# Job Application Tracker API

A secure and scalable RESTful API for managing job applications, built with ASP.NET Core, Entity Framework Core, and PostgreSQL. The API supports authentication, user-specific data access, and advanced querying features such as pagination, search, and sorting.

---

## Overview

This project is designed to simulate a real-world backend system where users can track their job applications. It demonstrates best practices in API design, authentication, and data handling.

---

## Features

* User registration and login (JWT authentication)
* Secure endpoints with authorization
* Create, read, update, and delete job applications
* Pagination support
* Search by company name
* Sort by application date
* User-specific data isolation

---

## Tech Stack

### Backend

* ASP.NET Core Web API (.NET 10)
* Entity Framework Core
* PostgreSQL

### Authentication

* JSON Web Tokens (JWT)

### API Documentation

* Swagger (Swashbuckle)

---

## Project Structure

* **Controllers** – API endpoints
* **Services** – business logic
* **Interfaces** – service contracts
* **DTOs** – data transfer objects
* **Models** – database entities
* **Data** – database context

---

## Getting Started

### Prerequisites

* .NET SDK (version 10 or later)
* PostgreSQL
* Node.js (optional, if testing with frontend later)

---

### Setup

1. Clone the repository:

```
git clone <https://github.com/raymondcjob/job-application-tracker>
cd JobApplicationTrackerApi
```

2. Configure the database connection in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=jobapplicationtrackerdb;Username=postgres;Password=yourpassword"
}
```

3. Apply migrations:

```
dotnet ef database update
```

4. Run the application:

```
dotnet run
```

5. Open Swagger:

```
http://localhost:5280/swagger
```

---

## Authentication

This API uses JWT for authentication.

### Register

`POST /api/Auth/register`

```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

### Login

`POST /api/Auth/login`

```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

### Using the Token

Click **Authorize** in Swagger and enter:

```
Bearer your_token_here
```

---

## API Endpoints

### Job Applications

* `GET /api/JobApplications`
* `GET /api/JobApplications/{id}`
* `POST /api/JobApplications`
* `PUT /api/JobApplications/{id}`
* `DELETE /api/JobApplications/{id}`

### Query Parameters

* `pageNumber`
* `pageSize`
* `companyName`
* `sortByDateDescending`

Example:

```
/api/JobApplications?pageNumber=1&pageSize=5&companyName=micro&sortByDateDescending=true
```

---

## Key Concepts Demonstrated

* RESTful API design
* Dependency injection
* Separation of concerns
* DTO pattern
* Authentication and authorization with JWT
* Database integration with Entity Framework Core
* Query optimization with pagination and filtering

---

## Future Improvements

* Input validation and error handling improvements
* Refresh token implementation
* Role-based authorization
* Unit and integration testing

---

## Author

Raymond Chau

---
