# EasyRoster Assessment

Production-aware customer management application built with **.NET 8, Angular, EF Core and SQL Server**.

## Stack

- ASP.NET Core Web API
- Angular
- EF Core + SQL Server
- OpenTelemetry
- Swagger / OpenAPI
- GitHub Actions CI

## Prerequisites

To run the solution locally, ensure the following are installed:

- .NET 8 SDK
- Node.js and npm
- Angular CLI
- SQL Server
- EF Core CLI (`dotnet-ef`)

## Features

- Customer CRUD
- Customer search
- Client and API validation
- Centralized error handling
- Structured logging
- Audit logging
- Rate limiting
- Restricted CORS
- OpenTelemetry tracing and metrics
- Automated tests

## Architecture

```text
Angular SPA
    ↓ HTTP / JSON (local development)
ASP.NET Core API
    ↓ EF Core
SQL Server
```

A layered monolith is used intentionally. Microservices, messaging, CQRS and distributed infrastructure were excluded to keep the solution proportional to the assessment.

The API and UI are maintained as separate projects, providing clear boundaries between frontend presentation concerns and backend application/data-access responsibilities.

## API

```text
GET     /api/customers?search={term}
GET     /api/customers/{id}
POST    /api/customers
PUT     /api/customers/{id}
DELETE  /api/customers/{id}
```

## Run

### Database

The application uses a SQL Server database named `EasyRosterDb`.

Database schema changes are managed through EF Core migrations included with the solution.

Configure `ConnectionStrings:DefaultConnection` using environment configuration or .NET user secrets.

```bash
dotnet user-secrets set \
  "ConnectionStrings:DefaultConnection" \
  "Server=localhost,1433;Database=EasyRosterDb;User Id=sa;Password=<YOUR_SQL_SERVER_PASSWORD>;TrustServerCertificate=True;Encrypt=False" \
  --project api/EasyRoster.Api/EasyRoster.Api.csproj
```

Apply the included migrations:

```bash
dotnet ef database update \
  --project api/EasyRoster.Api/EasyRoster.Api.csproj \
  --startup-project api/EasyRoster.Api/EasyRoster.Api.csproj
```

### API

From the repository root:

```bash
dotnet run --project api/EasyRoster.Api/EasyRoster.Api.csproj
```

API:

```text
http://localhost:5000
```

### Angular

```bash
cd client/easy-roster-ui
npm ci
npm start
```

UI:

```text
http://localhost:4200
```

The Angular application communicates with the ASP.NET Core API, with customer search performed server-side through the customer API.

## Tests

### Backend

From the repository root:

```bash
dotnet test api/EasyRoster.Api.Tests/EasyRoster.Api.Tests.csproj
```

### Frontend

```bash
cd client/easy-roster-ui
npm test -- --watch=false
```

### Verification

The solution was verified locally before submission:

- API Release build: successful
- Angular production build: successful
- API tests: **9/9 passing**
- Angular tests: **10/10 passing**
- Total automated tests: **19/19 passing**

## Build

### API

```bash
dotnet build api/EasyRoster.Api/EasyRoster.Api.csproj -c Release
```

### Angular

```bash
cd client/easy-roster-ui
npm ci
npm run build
```

## Technical Notes

- DTOs isolate API contracts from persistence models.
- API validation is enforced independently of Angular validation