# AGENTS.md - React Admin Panel

## Commands
```bash
npm run dev       # Start dev server (Vite)
npm run build     # Type-check (tsc -b) + build (vite build)
npm run lint      # ESLint (flat config, TypeScript + React Hooks + React Refresh)
npm run preview   # Preview production build
```

**Order matters**: `npm run build` runs `tsc -b` (typecheck) before `vite build`. Run `lint` before `build` for CI.

## Environment
```bash
# .env (required)
VITE_API_URL=https://localhost:7071/api
```
Backend API defaults to `https://localhost:7071/api` (.NET backend).

## Tech Stack
- **React 19** + **TypeScript** (strict: `noUnusedLocals`, `noUnusedParameters`, `erasableSyntaxOnly`)
- **Vite 8** with **React Compiler** (`babel-plugin-react-compiler`)
- **Tailwind CSS 4** + **DaisyUI 5** (via `@tailwindcss/vite`)
- **React Router 7** (router provider pattern)
- **TanStack Query 5** (server state)
- **React Hook Form 7** + **Zod 4** (forms/validation)
- **Axios** (HTTP), **Lucide React** (icons), **Recharts** (charts), **React Toastify** (toasts)

## Project Structure
```
src/
├── app/
│   ├── layouts/        # MainLayout, Sidebar, Navbar
│   └── router/         # Router provider, ProtectedRoute, AdminRoute
├── features/
│   ├── dashboard/      # Dashboard page + widgets
│   ├── projects/       # Projects CRUD
│   └── users/          # Users CRUD
├── pages/              # LoginPage
└── shared/
    ├── components/     # DataTable, Modal, FormInput, etc.
    ├── context/        # AuthContext
    ├── hooks/          # useProjects, useUsers, useAuth, useDashboard, useErrors
    ├── services/       # API services (project, user)
    ├── types/          # TypeScript interfaces/DTOs
    └── utils/          # api.ts (axios), toast.config, formatToIranTime, theme toggle
```

## Key Conventions
- **React Compiler** enabled via Vite plugin (no manual `useMemo`/`useCallback` needed)
- **Strict TypeScript**: unused locals/params are errors; `erasableSyntaxOnly` enforced
- **Path aliases**: Not configured — use relative imports
- **API base URL**: `import.meta.env.VITE_API_URL` via `src/shared/utils/api.ts`
- **Auth**: JWT in localStorage via `AuthContext`; protected routes via `ProtectedRoute`/`AdminRoute`
- **Forms**: React Hook Form + Zod resolvers (`@hookform/resolvers/zod`)
- **Tables**: Reusable `DataTable` component with sorting, pagination, search
- **Styling**: Tailwind 4 + DaisyUI 5 (no config file — uses `@theme` in CSS)

## Testing
No test framework configured. No test scripts in `package.json`.

## Backend API
Expects .NET API at `VITE_API_URL`:
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/users` | Create user |
| GET | `/api/users` | List users (paged) |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/projects` | Create project |
| GET | `/api/projects/user` | List projects (paged) |
| GET | `/api/projects/{id}` | Get project by ID |
| PUT | `/api/projects` | Update project |
| DELETE | `/api/projects/{id}` | Delete project |

Default credentials: `admin/admin` (Admin), `user/user` (User)

## Gotchas
- **No path aliases** — use relative imports (`../../shared/utils/api`)
- **TypeScript strictness** — unused imports/variables are errors; `erasableSyntaxOnly` forbids enums, namespaces, etc.
- **React Compiler** — don't add manual memoization; it handles optimization
- **Tailwind 4** — uses `@import "tailwindcss"` in CSS, not `tailwind.config.js`
- **Vite builds** run `tsc -b` first — type errors fail the build
- **No test runner** — add Vitest/Jest if needed