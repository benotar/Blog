# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Full-stack blog application: ASP.NET Core 10 backend + React 18/Vite frontend, orchestrated with Docker Compose.

## Commands

### Full stack
```bash
docker compose up       # Start PostgreSQL, backend (5000), frontend (5173)
```

### Backend (`src/Blog.Backend/`)
```bash
dotnet build Blog.Backend.sln
dotnet run --project Blog.API/Blog.API.csproj
dotnet test Blog.Backend.sln                                        # Run all tests
dotnet test --filter "FullyQualifiedName~UserServiceTests"          # Run a single test class
dotnet ef migrations add <Name> --project Blog.Persistence --startup-project Blog.API
```

### Frontend (`src/Blog.Frontend/`)
```bash
npm install
npm run dev      # Dev server on port 5173
npm run build
npm run lint
```

## Backend Architecture (Clean Architecture)

Four projects with strict dependency direction: Domain ← Application ← Persistence ← API.

- **Blog.Domain**: Entities (`User`, `Post`, `Comment`, `Like`, `RefreshToken`) and enums (`ErrorCode`, `PostCategory`, `JwtType`). No dependencies.
- **Blog.Application**: Service interfaces + implementations (`UserService`, `PostService`, `CommentService`, `GoogleService`), DTOs (Request/Response models), `JwtProvider`, `PasswordHasher`. Services return `Result<T>` — never throw for business errors.
- **Blog.Persistence**: EF Core `ApplicationDbContext` (PostgreSQL via Npgsql), generic `Repository<T>`, `UnitOfWork`. All DB access goes through `IUnitOfWork`.
- **Blog.API**: Controllers, DI wiring, global exception handler middleware, request logging middleware. Uses Scalar (not Swagger UI) for API docs at `/scalar`.

### Key patterns
- **`Result<T>`**: All service methods return this; controllers map error codes to HTTP status codes.
- **`IUnitOfWork`**: Single entry point for all repositories; call `SaveChangesAsync()` explicitly.
- **Generic `IRepository<T>`**: Provides `GetAll`, `GetByFilter`, `Add`, `Update`, `Delete`. Specialized queries are added as extension methods or extra interfaces, not by breaking the generic.
- **Centralized NuGet versions**: `Directory.Packages.props` — add versions there, reference without version in `.csproj`.
- **Auth**: JWT Bearer (15 min access token) + refresh tokens (30 days) stored in DB. Roles: `Admin`, `User`.

## Frontend Architecture

- **Routing**: React Router v7, protected by `PrivateRoute` / `OnlyAdminPrivateRoute` wrappers.
- **State**: Zustand stores in `src/zustand/` — auth tokens, current user, theme.
- **HTTP**: Axios instance in `src/axios/` with interceptors that silently refresh JWT on 401 before retrying.
- **UI**: Tailwind CSS 3 + Flowbite React components.
- **Rich text**: React Quill on create/edit post pages.
- **Proxy**: Vite dev server proxies `/api` → backend, so no CORS config needed locally.

## Testing

Tests live in `Blog.Tests` and use **xUnit + Moq + FluentAssertions + Testcontainers** (spins up a real PostgreSQL container for integration tests).

- Unit tests mock `IUnitOfWork` / `IRepository<T>` via Moq.
- Integration tests use `Testcontainers.PostgreSql` and a real `ApplicationDbContext`.

## CI/CD

- **`backend-ci.yml`**: Runs on PR to `main` and pushes to non-`main` branches — restores, builds Release, runs tests (.NET 10.x).
- **`docker-build-push.yml`**: Triggers on push to `main` — builds both images and pushes to Docker Hub (`benotar/*`) tagged `latest` + commit SHA.
