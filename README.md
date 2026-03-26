# Job Application Tracker API

Backend system designed to manage job application workflows, including user authentication, secure data access, and structured querying features such as pagination, search, and sorting.

---

## Overview

This project demonstrates the development of a secure and scalable RESTful API using ASP.NET Core with JSON-based request and response handling. It simulates a real-world system where users manage and track application data while ensuring proper access control and data isolation.

---

## Features

- User registration and login (JWT authentication)
- Secure endpoints with authorization
- Create, read, update, and delete job applications
- Pagination support
- Search by company name
- Sort by application date
- User-specific data isolation

---

## Tech Stack

### Backend

- C#
- ASP.NET Core Web API (.NET 10)
- Entity Framework Core
- PostgreSQL

### Authentication

- JSON Web Tokens (JWT)

### Tools

- Swagger (API testing)
- Git

---

## Technical Skills

This project demonstrates:

- Writing, modifying, and testing software code
- Developing RESTful APIs using JSON-based communication
- Implementing secure system design with authentication and data access control
- Managing system workflows and data usage
- Ensuring reliability through structured querying and validation

---

## Testing & Validation

- API endpoints tested using Postman and Swagger
- Authentication and authorization flows validated
- Query parameters tested for pagination, filtering, and sorting
- Invalid inputs handled to maintain system stability

---

## Project Structure

- Controllers – API endpoints
- Services – business logic
- Interfaces – service contracts
- DTOs – data transfer objects
- Models – database entities
- Data – database context

---

## Authentication

This API uses JWT for authentication.

### Register

POST /api/Auth/register

```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

### Login

POST /api/Auth/login

```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

### Using the Token

```text
Bearer your_token_here
```

---

## API Endpoints

### Job Applications

- GET /api/JobApplications
- GET /api/JobApplications/{id}
- POST /api/JobApplications
- PUT /api/JobApplications/{id}
- DELETE /api/JobApplications/{id}

### Query Parameters

- pageNumber
- pageSize
- companyName
- sortByDateDescending

Example:

```text
/api/JobApplications?pageNumber=1&pageSize=5&companyName=micro&sortByDateDescending=true
```

---

## Key Concepts Demonstrated

- RESTful API design
- JSON-based communication
- Dependency injection
- Separation of concerns
- DTO pattern
- Authentication and authorization with JWT
- Database integration with Entity Framework Core
- Query optimization with pagination and filtering

---

## Future Improvements

- Enhanced input validation and global error handling
- Refresh token implementation
- Role-based authorization
- Unit and integration testing

---

## Author

Raymond Chau