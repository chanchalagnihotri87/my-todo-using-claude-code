# MyTodo — Personal Assistant

A personal assistant web application for managing your whole life through a structured hierarchy: **Life Areas → Problems → Solutions → Objectives → Tasks → Sprint → Done**. Solutions can also spin off **Experiments** to validate ideas, and Tasks can be pulled into a daily **Todo** list.

Built with ASP.NET Core MVC on .NET 10, following Clean Architecture.

```
Life Area
 └─ Problem            (Pending, WorkingOnIt, Resolved, Discarded)
     └─ Solution        (Planned, Verifying, Verified, AddedInRoutine, BecomeSecondNature, Discarded)
         ├─ Objective   (NotStarted, InProgress, Completed)
         │   └─ Task    (Pending, InProgress, Completed) — can be assigned to a Sprint
         └─ Experiment  (Innovation, Verifying, Verified, AddedInSOP, Discarded)
```

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC, .NET 10 |
| Data access | Entity Framework Core 10 (SQL Server) |
| Logging | Serilog (file sink) |
| Frontend | Razor Views, jQuery |

## Quick Start

```bash
dotnet ef database update --project MyTodo.Infrastructure --startup-project MyTodo
dotnet run --project MyTodo
```

Full setup instructions (prerequisites, connection string, verifying it works): [`docs/getting-started.md`](docs/getting-started.md).

## Documentation

| Doc | Covers |
|---|---|
| [`docs/getting-started.md`](docs/getting-started.md) | Prerequisites, setup, running, everyday commands |
| [`docs/architecture.md`](docs/architecture.md) | Clean Architecture layers, request flow, design patterns |
| [`docs/database.md`](docs/database.md) | Full schema, entity relationships, migration history |
| [`docs/features.md`](docs/features.md) | What every page/controller does, in detail |
| [`docs/logging.md`](docs/logging.md) | Serilog setup and the controller logging pattern |
| [`docs/error-handling.md`](docs/error-handling.md) | The global exception handler |

Each project also has its own `CLAUDE.md` documenting its specific coding conventions:
- [`MyTodo/CLAUDE.md`](MyTodo/CLAUDE.md) — controller/view/model rules, DTO vs. ViewModel usage
- [`MyTodo.Application/CLAUDE.md`](MyTodo.Application/CLAUDE.md) — service/repository layering, SOLID conventions
- [`MyTodo.Infrastructure/CLAUDE.md`](MyTodo.Infrastructure/CLAUDE.md) — persistence folder layout and conventions

## Project Structure

```
MyTodo/                        # Presentation (ASP.NET Core MVC) — no .sln, build via .csproj files
├── Controllers/
├── Views/
├── Models/                    # Input ViewModels only
├── Middleware/                 # GlobalExceptionHandler
├── Helpers/                    # Status → badge/text display helpers
└── wwwroot/                    # css, js, static assets
MyTodo.Application/             # Services, DTOs, interfaces
MyTodo.Domain/                  # Entities, enums
MyTodo.Infrastructure/          # DbContext, EF configurations, migrations, repositories
└── Persistence/
    ├── Configurations/
    ├── DbContext/
    ├── Migrations/
    └── Repositories/
docs/                           # Detailed documentation (see table above)
```
