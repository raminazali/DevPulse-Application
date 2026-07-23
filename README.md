# DevPulse

An error tracking and monitoring platform with an ASP.NET Core 10 backend API and a React 19 admin dashboard. Capture, track, and resolve application errors with project management, user roles, and real-time monitoring.

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started — Development](#getting-started--development)
  - [Backend (API)](#backend-api)
  - [Frontend (Admin Panel)](#frontend-admin-panel)
- [Getting Started — Docker](#getting-started--docker)
- [API Endpoints](#api-endpoints)
- [Environment Variables](#environment-variables)
- [CI/CD Pipeline](#cicd-pipeline)
- [License](#license)

---

## Architecture Overview

```
┌──────────────┐     ┌──────────────────┐     ┌────────────┐
│  React 19    │────▶│  ASP.NET Core 10  │────▶│ PostgreSQL │
│  Admin Panel │     │  Web API          │     │            │
│  (port 3000) │     │  (port 5000)      │     │  (port 5432)│
└──────────────┘     └──────────────────┘     └────────────┘
       │                       │
       └── Nginx proxy ────────┘
           /api/ → api:8080/api/
```

The frontend is served by Nginx, which also proxies `/api/` requests to the backend so the browser never needs to know internal Docker hostnames.

---

## Tech Stack

### Backend
- **.NET 10.0** (ASP.NET Core Web API, C# 13)
- **Entity Framework Core 10** with Npgsql (PostgreSQL)
- **JWT Bearer** + **API Key** dual authentication
- **Ardalis.Specification** for query patterns
- **Scalar** for API reference UI
- **AWS S3** for screenshot storage (optional)
- **xUnit** + **Moq** + **FluentAssertions** for testing

### Frontend
- **React 19** with TypeScript (strict mode, React Compiler via Babel)
- **Vite 8** for build tooling
- **Tailwind CSS 4** + **DaisyUI 5** for styling
- **React Router 7** for routing
- **TanStack Query 5** for server state
- **React Hook Form 7** + **Zod 4** for form validation
- **Axios** for HTTP, **Recharts** for charts, **React Toastify** for toasts

---

## Project Structure

```
Partner/
├── DevPulse/                          # Backend (.NET 10)
│   ├── DevPulse.slnx
│   ├── DevPulse.Api/                  # Presentation layer
│   │   ├── Controllers/               # API endpoints
│   │   ├── Middleware/                 # Global exception handler
│   │   ├── Authentication/            # JWT + API Key handlers
│   │   ├── Program.cs                 # App entry point
│   │   └── Dockerfile
│   ├── DevPulse.Application/          # DTOs, Services, Specifications
│   ├── DevPulse.Core/                 # Entities, Enums, Interfaces
│   └── DevPulse.Infrastructure/       # EF Core DbContext, Migrations, AWS S3
│
├── admin-panel/                       # Frontend (React 19)
│   ├── src/
│   │   ├── app/layouts/               # MainLayout, Sidebar, Navbar
│   │   ├── app/router/                # Route definitions, ProtectedRoute
│   │   ├── features/                  # Dashboard, Projects, Users
│   │   ├── pages/                     # LoginPage
│   │   └── shared/                    # Components, hooks, services, utils
│   ├── Dockerfile
│   └── nginx.conf                     # Reverse proxy config
│
├── docker-compose.yml                 # Local development
├── docker-compose.prod.yml            # Production deployment
└── .github/workflows/ci-cd.yml        # CI/CD pipeline
```

---

## Getting Started — Development

### Backend (API)

**Prerequisites:** .NET 10.0 SDK, PostgreSQL server.

```bash
cd DevPulse

# Update connection string in appsettings.json, then:
dotnet run --project DevPulse.Api
```

The API starts at `http://localhost:5091`. Scalar API reference is at `/scalar` in development mode.

**Run tests:**
```bash
dotnet test
```

### Frontend (Admin Panel)

**Prerequisites:** Node.js 18+, running backend API.

```bash
cd admin-panel
npm install

# Edit .env if your backend runs on a different URL:
# VITE_API_URL=https://localhost:7071/api

npm run dev        # Development server
npm run build      # TypeScript check + Vite build
npm run lint       # ESLint
```

---

## Getting Started — Docker

### Development (with hot-reload not available)

```bash
docker compose up -d
```

| Service  | URL                     | Description          |
|----------|-------------------------|----------------------|
| **web**  | http://localhost:3000   | React admin panel    |
| **api**  | http://localhost:5000   | .NET backend         |
| **db**   | localhost:5432          | PostgreSQL           |

After code changes, rebuild a specific service:
```bash
docker compose up -d --build api
```

### Production

1. Set up secrets in your GitHub repository (see [CI/CD](#cicd-pipeline)).
2. On your server:
```bash
git clone <repo> /opt/myproject
cd /opt/myproject
echo "DB_PASSWORD=your_secure_password" >> .env
docker compose -f docker-compose.prod.yml up -d
```

3. Push to `main` — the CI/CD pipeline automatically builds, pushes to Docker Hub, and deploys.

### Default Credentials

| Role    | Email    | Password |
|---------|----------|----------|
| Admin   | admin    | admin    |
| User    | user     | user     |

---

## API Endpoints

### Authentication

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/login` | Login and get JWT token | None |

Rate limited: 5 req/min per IP.

### Users (Admin only)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/users` | Create user |
| GET | `/api/users` | List users (paginated) |
| GET | `/api/users/{id}` | Get user by ID |

### Projects

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/projects/{id}` | Get project by ID |
| GET | `/api/projects/user` | List current user's projects |
| POST | `/api/projects` | Create project |
| PUT | `/api/projects` | Update project |
| DELETE | `/api/projects/{id}` | Delete project |

### Errors

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/errors` | List errors (filterable) |
| GET | `/api/errors/{id}` | Error detail |

**Filters:** `ProjectId`, `ExceptionType`, `Search`, `Page`, `PageSize`

### External Errors (API Key)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/external/errors` | Report error from external app |

Rate limited: 25 req/min per IP. Requires `X-Api-Key` header.

### Dashboard

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/dashboard` | Aggregated error stats & trends |

Cached for 3 minutes.

---

## Environment Variables

### Backend (`appsettings.json`)

| Key | Description |
|-----|-------------|
| `ConnectionStrings:DevPulse` | PostgreSQL connection string |
| `JwtSettings:Key` | JWT signing key (min 32 chars) |
| `JwtSettings:Issuer` | Token issuer |
| `JwtSettings:Audience` | Token audience |
| `S3Settings:BucketName` | AWS S3 bucket (optional) |
| `S3Settings:Region` | AWS region |
| `S3Settings:AccessKey` | AWS access key |
| `S3Settings:SecretKey` | AWS secret key |

### Frontend (`.env`)

| Variable | Default | Description |
|----------|---------|-------------|
| `VITE_API_URL` | `https://localhost:7071/api` | Backend API base URL |

### Docker

| Variable | Used In | Description |
|----------|---------|-------------|
| `DB_PASSWORD` | `docker-compose.prod.yml` | PostgreSQL password |

---

## CI/CD Pipeline

On every push to `main`, GitHub Actions runs:

1. **build-and-test** — `dotnet restore` → `build` → `test`
2. **build-and-push-docker** — Builds API image, pushes to Docker Hub
3. **deploy** — SSHes into server, pulls new image, restarts stack

### Required GitHub Secrets

| Secret | Purpose |
|--------|---------|
| `DOCKER_HUB_USERNAME` | Docker Hub login |
| `DOCKER_HUB_TOKEN` | Docker Hub access token |
| `SERVER_IP` | Deployment server IP |
| `SERVER_USERNAME` | SSH username |
| `SSH_PRIVATE_KEY` | SSH private key |

---

## License

MIT License. See [LICENSE](DevPulse/LICENSE) for details.
