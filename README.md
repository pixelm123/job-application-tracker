# Job Application Tracker

A full-stack job search management app built as a portfolio project. Track applications, manage interview stages via a Kanban board, attach CVs, and receive email reminders.

## Tech Stack

**Backend**
- .NET 10, ASP.NET Core, Clean Architecture (Domain / Application / Infrastructure / API)
- CQRS with MediatR, FluentValidation pipeline
- EF Core 10 + Npgsql + PostgreSQL
- ASP.NET Core Identity + JWT authentication
- Redis distributed cache (graceful fallback if unavailable)
- Hangfire background jobs (daily reminder emails via SMTP)
- Scalar API docs

**Frontend**
- Angular 18 (standalone components, signals)
- Tailwind CSS v3
- Angular CDK drag-and-drop (Kanban board)
- Reactive Forms with inline validation

**Infrastructure**
- Docker Compose for local PostgreSQL, Redis, and Mailpit (SMTP)
- Dockerfile for Render deployment

## Features

- Register and login with JWT-secured sessions
- Create, edit, and delete job applications
- Track status: Applied, Interview, Offer, Rejected
- Drag-and-drop Kanban board to move applications between stages
- Dashboard with status counts, success rate, and recent activity
- Upload and download CV (PDF) per application
- Set reminder dates - receive an email notification on the day
- Paginated application list with search and status filter

## Local Setup

### Prerequisites

- .NET 10 SDK
- Node.js 18+
- Docker Desktop

### 1. Clone the repo

```bash
git clone <repo-url>
cd job-application-tracker
```

### 2. Start infrastructure

```bash
docker-compose up -d
```

This starts PostgreSQL on port 5432, Redis on port 6379, and Mailpit on port 1025. The Mailpit web UI is at http://localhost:8025.

### 3. Configure the API

Copy the example settings file:

```bash
cp JobTracker.API/appsettings.Development.json.example JobTracker.API/appsettings.Development.json
```

The defaults match the Docker Compose setup, so no changes are needed for local dev.

### 4. Run the API

```bash
cd JobTracker.API
dotnet run
```

The API starts on http://localhost:5293. Database migrations run automatically on startup. API docs are at http://localhost:5293/scalar.

### 5. Run the frontend

```bash
cd frontend
npm install
npm start
```

The app is available at http://localhost:4200.

## Project Structure

```
JobTracker.Domain/          - Entities, enums, domain exceptions
JobTracker.Application/     - CQRS handlers, validators, interfaces
JobTracker.Infrastructure/  - EF Core, Identity, Redis, email, Hangfire
JobTracker.API/             - Controllers, middleware, Program.cs
JobTracker.Tests/           - Unit tests (domain + handler tests)
frontend/                   - Angular 18 application
docker-compose.yml          - Local dev infrastructure
```

## Running Tests

```bash
dotnet test
```
