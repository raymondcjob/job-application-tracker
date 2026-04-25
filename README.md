# Job Application Tracker

Full-stack job application tracker with an ASP.NET Core API backend and a React frontend for managing the job search pipeline in a cleaner dashboard experience.

---

## Overview

This project demonstrates a secure and scalable application for tracking job applications. The backend exposes a RESTful API with JWT authentication and protected CRUD endpoints, while the frontend provides a React interface for signing in, filtering results, and creating or updating job applications.

---

## Features

- User registration and login with JWT authentication
- Role support for `user` and `admin`
- Default seeded admin account for local access
- Protected job application CRUD flows
- Pagination support
- Company-name search
- Sorting by application date
- User-specific data isolation
- React dashboard for authentication, filtering, status tracking, and editing
- Automatic archiving for unanswered applications after 14 days

---

## Tech Stack

### Backend

- C#
- ASP.NET Core Web API (.NET 10)
- Entity Framework Core
- PostgreSQL

### Frontend

- React
- Vite
- Vanilla CSS

### Authentication

- JSON Web Tokens (JWT)

---

## Project Structure

- `JobApplicationTrackerApi/Controllers` - API endpoints
- `JobApplicationTrackerApi/Services` - business logic
- `JobApplicationTrackerApi/Interfaces` - service contracts
- `JobApplicationTrackerApi/DTOs` - data transfer objects
- `JobApplicationTrackerApi/Models` - database entities
- `JobApplicationTrackerApi/Data` - database context
- `job-application-tracker-client/src` - React frontend

---

## Running The App

### Backend

1. Start PostgreSQL and make sure your connection string and JWT settings are configured in `JobApplicationTrackerApi/appsettings.json`.
2. Run the API:

```bash
dotnet run --project JobApplicationTrackerApi
```

The API runs on `http://localhost:5280` in development.

### Frontend

1. Install dependencies:

```bash
npm install
```

2. Start the React app:

```bash
npm run dev
```

The frontend runs on `http://localhost:5173` by default and proxies `/api` requests to the backend.

If you want to point the frontend at a different API URL, create a `.env` file in `job-application-tracker-client` with:

```bash
VITE_API_URL=http://localhost:5280
```

---

## Authentication

The frontend uses the same JWT authentication flow as the API. Successful login and registration responses return a token that the React app stores locally and sends with protected requests.

### Register

POST `/api/Auth/register`

```json
{
  "emailOrUsername": "user@example.com",
  "password": "password123"
}
```

### Login

POST `/api/Auth/login`

```json
{
  "emailOrUsername": "user@example.com",
  "password": "password123"
}
```

### Default Admin

The application seeds a default admin account at startup:

- Username: `admin`
- Password: `adminpwd`

Registrations created from the frontend are always standard `user` accounts.

### Using the Token

```text
Bearer your_token_here
```

---

## API Endpoints

### Job Applications

- GET `/api/JobApplications`
- GET `/api/JobApplications/{id}`
- POST `/api/JobApplications`
- PUT `/api/JobApplications/{id}`
- DELETE `/api/JobApplications/{id}`

### Query Parameters

- `pageNumber`
- `pageSize`
- `companyName`
- `sortByDateDescending`

Example:

```text
/api/JobApplications?pageNumber=1&pageSize=5&companyName=micro&sortByDateDescending=true
```

---

## Future Improvements

- Refresh token support
- Better form validation and global error handling
- Server-hosted production frontend build
- Unit and integration tests for frontend and backend

---

## Author

Raymond Chau
