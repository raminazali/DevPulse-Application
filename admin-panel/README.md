# DevPulse Admin Panel

A React-based admin dashboard for managing users, projects, and error monitoring, connected to a .NET backend API.

## Tech Stack

- **React 19** with TypeScript (strict mode, React Compiler via Babel)
- **Vite 8** for build tooling
- **Tailwind CSS 4** + **DaisyUI 5** for styling
- **React Router 7** for routing (data router pattern)
- **TanStack Query 5** for server state management
- **React Hook Form 7** + **Zod 4** for form validation
- **Axios** for HTTP requests
- **React Toastify** for toast notifications
- **Recharts** for charts
- **Lucide React** for icons

## Features

- **Authentication** — Login/logout with JWT, protected routes, token-based API auth
- **Dashboard** — Summary stats, charts (hourly errors, project distribution, top exception types), recent errors, top users, admin overview
- **Users** — List with pagination, search, sorting, and create user modal with async-select
- **Projects** — Full CRUD with list, create, edit, delete, pagination, search, sorting, and status badges
- **Error Tracking** — Error group listing and detail view with filtering
- **Data Table** — Reusable generic table with sorting, pagination, search, page size selector, export buttons, and row actions (edit/view/delete)
- **Modals** — Reusable modal component with portal rendering, Escape key support, and focus restoration
- **Form Components** — Input, Select, Switch, Textarea, AsyncSelect with Zod validation and react-hook-form integration
- **Theme Toggle** — Light/dark mode with persistent localStorage preference, no flash on load
- **RTL Support** — Full Persian (Farsi) UI with right-to-left layout and Persian date formatting

## Project Structure

```
src/
├── app/
│   ├── layouts/        # MainLayout, Sidebar, Navbar
│   └── router/         # Route definitions, ProtectedRoute
├── features/
│   ├── dashboard/      # Dashboard page with charts, stats, tables
│   ├── projects/       # Projects CRUD (list, create, edit, delete)
│   └── users/          # Users CRUD (list, create)
├── pages/              # LoginPage
└── shared/
    ├── components/     # DataTable, Modal, FormInput, FormAsyncSelect, etc.
    ├── context/        # AuthContext + AuthProvider
    ├── hooks/          # useAuth, useProjects, useUsers, useDashboard, useErrors
    ├── services/       # API service modules (auth, project, user, dashboard, errors)
    ├── types/          # TypeScript interfaces and DTOs
    └── utils/          # Axios client, date formatting, theme toggle, toast config
```

## Getting Started

### Prerequisites

- Node.js 18+
- A running .NET backend API (default: `https://localhost:7071/api`)

### Installation

```bash
npm install
```

### Environment

Create a `.env` file in the project root:

```
VITE_API_URL=https://localhost:7071/api
```

### Development

```bash
npm run dev
```

### Build

```bash
npm run build     # tsc -b + vite build
```

### Lint

```bash
npm run lint
```

### Preview Production Build

```bash
npm run preview
```

## API Endpoints

The app consumes a REST API with the following endpoints:

| Method | Endpoint              | Description              |
|--------|-----------------------|--------------------------|
| POST   | `/api/Auth/login`     | Authenticate user        |
| GET    | `/api/users`          | List users (paged)       |
| GET    | `/api/users/{id}`     | Get user by ID           |
| POST   | `/api/users`          | Create user              |
| GET    | `/api/projects/user`  | List projects (paged)    |
| GET    | `/api/projects/{id}`  | Get project by ID        |
| POST   | `/api/projects`       | Create project           |
| PUT    | `/api/projects`       | Update project           |
| DELETE | `/api/projects/{id}`  | Delete project           |
| GET    | `/api/dashboard`      | Dashboard report         |
| GET    | `/api/errors`         | List errors (paged)      |
| GET    | `/api/errors/{id}`    | Get error detail         |

## Environment Variables

| Variable        | Default                           | Description   |
|-----------------|-----------------------------------|---------------|
| `VITE_API_URL`  | `https://localhost:7071/api`      | Backend API   |
