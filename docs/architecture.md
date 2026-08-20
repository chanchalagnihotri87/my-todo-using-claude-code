# Architecture

MyTodo is an ASP.NET Core MVC personal-assistant app built on **Clean Architecture**. Every piece of user data flows through one hierarchy:

```
LifeArea → Problem → Solution → Objective → TodoTask → Sprint / Todo
                            └──→ Experiment
```

(the "manage your whole life" pipeline described in the project's root `CLAUDE.md`). The codebase is split into five projects so that this domain model and its business rules stay independent of ASP.NET Core, EF Core, and SQL Server.

## Solution Layout

```
MyTodo.sln
├── MyTodo.Domain.Shared/   enums only — no dependencies
├── MyTodo.Domain/          entities — plain C#, no EF/ASP.NET attributes
├── MyTodo.Application/     services, DTOs, repository interfaces — the use-case layer
├── MyTodo.Infrastructure/  EF Core DbContext, entity configs, migrations, repository implementations
└── MyTodo/                 ASP.NET Core MVC — controllers, Razor views, ViewModels, composition root
```

Each project has a `CLAUDE.md` with layer-specific rules: [`MyTodo/CLAUDE.md`](../MyTodo/CLAUDE.md), [`MyTodo.Application/CLAUDE.md`](../MyTodo.Application/CLAUDE.md), [`MyTodo.Infrastructure/CLAUDE.md`](../MyTodo.Infrastructure/CLAUDE.md).

## Layer Dependency Graph

Dependencies point **inward only** — outer layers know about inner layers, never the reverse:

```
┌─────────────────────────────────────────────────────────────────┐
│ MyTodo  (presentation / composition root)                       │
│   Controllers · Views · ViewModels · Middleware · Program.cs    │
└───────────────┬───────────────────────────────────┬─────────────┘
                 │ depends on                        │ composition root only
                 ▼                                   ▼
┌─────────────────────────────┐        ┌─────────────────────────────────┐
│ MyTodo.Application           │◀───────│ MyTodo.Infrastructure            │
│   Services · Service         │ implements │ DbContext · EF Configurations │
│   interfaces · DTOs ·        │ repository │ Migrations · Repository impls │
│   Repository interfaces      │ interfaces │                               │
└───────────────┬──────────────┘        └───────────────┬───────────────────┘
                 │ depends on                            │ depends on
                 ▼                                       │
┌─────────────────────────────┐                          │
│ MyTodo.Domain                 │◀─────────────────────────┘
│   Entities                    │
└───────────────┬───────────────┘
                 │ depends on
                 ▼
┌─────────────────────────────┐
│ MyTodo.Domain.Shared          │
│   Enums                       │
└───────────────────────────────┘
```

| Layer | Contains | Depends on |
|---|---|---|
| `MyTodo.Domain.Shared` | `Enums/` — `ProblemStatus`, `SolutionStatus`, `ObjectiveStatus`, `TodoStatus`, `ExperimentStatus` | *(nothing)* |
| `MyTodo.Domain` | `Entities/` — `LifeArea`, `Problem`, `Solution`, `Objective`, `TodoTask`, `Todo`, `Sprint`, `Experiment`, `ProblemStatusOrder` | `MyTodo.Domain.Shared` |
| `MyTodo.Application` | `Services/`, `Services/Interfaces/`, `Services/Common/` (e.g. `ReorderHelper`), `DTOs/`, `Repositories/Interfaces/`, `DependencyInjection.cs` | `MyTodo.Domain` |
| `MyTodo.Infrastructure` | `Persistence/DbContext/`, `Persistence/Configurations/` (Fluent API, one per entity), `Persistence/Migrations/`, `Persistence/Repositories/`, `DependencyInjection.cs` | `MyTodo.Domain`, `MyTodo.Application` (interfaces only) |
| `MyTodo` | `Controllers/`, `Views/`, `Models/` (input ViewModels + request DTOs), `Extensions/`, `Helpers/`, `Middleware/`, `Program.cs` | `MyTodo.Application`, `MyTodo.Domain.Shared`, `MyTodo.Infrastructure` (composition root only) |

**Why the presentation project references `MyTodo.Infrastructure` at all:** `Program.cs` is the app's composition root and is the one place allowed to see every layer, because it has to call `AddInfrastructureServices()` to register the `DbContext` and repositories. Controllers, Views, and Models never use that reference — they depend only on service interfaces from `MyTodo.Application`. This is enforced by convention (see `MyTodo/CLAUDE.md`), not by the compiler.

**What this buys:**
- `MyTodo.Infrastructure` could be swapped for a different database/ORM without touching `Application` or `Domain`.
- `MyTodo.Application`'s business logic can be unit tested by mocking `Repositories/Interfaces/*`, with no real database involved.
- `MyTodo.Domain` entities have zero framework attributes — all persistence mapping lives in `Infrastructure/Persistence/Configurations/*Configuration.cs` (Fluent API), never `[Table]`/`[Column]` attributes on the entity itself.

## Composition Root (`Program.cs`)

```csharp
builder.Host.UseSerilog(...)                                    // structured logging — see logging.md
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");
builder.Services.AddApplicationServices();                       // MyTodo.Application/DependencyInjection.cs
builder.Services.AddInfrastructureServices(builder.Configuration); // MyTodo.Infrastructure/DependencyInjection.cs
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();  // see error-handling.md
builder.Services.AddProblemDetails();
```

`AddApplicationServices()` and `AddInfrastructureServices()` are extension methods that each layer owns and exposes, keeping `Program.cs` a short list of *what* to wire up rather than *how*. Both register everything as **scoped** (one instance per HTTP request) — appropriate since services and repositories hold a reference to the request-scoped `DbContext`.

Default route is `{controller=LifeAreas}/{action=Index}/{id?}` — the app opens on the Life Areas list, not a dashboard.

## Request Flow

A typical write request (e.g. creating an Objective) flows through the layers like this:

```
Browser (jQuery $.ajax, JSON body, X-CSRF-TOKEN header)
   │
   ▼
Controller action                          MyTodo/Controllers/ObjectivesController.cs
   │  - [ValidateAntiForgeryToken], ModelState checks
   │  - maps ViewModel/Request → DTO
   │  - logs Warning on invalid input / not-found (see logging.md)
   ▼
Service                                    MyTodo.Application/Services/ObjectiveService.cs
   │  - implements IObjectiveService
   │  - owns business rules (SortOrder assignment, CompletedAt stamping, ...)
   │  - maps Entity → DTO for the response
   ▼
Repository                                 MyTodo.Infrastructure/Persistence/Repositories/ObjectiveRepository.cs
   │  - implements IObjectiveRepository, extends BaseRepository<T>
   │  - talks to MyTodoDbContext (EF Core)
   ▼
SQL Server (MyTodoDB_GenerateClaudeMdByOwn)
```

Reads follow the same path but skip DTO→Entity mapping, and `BaseRepository<T>.GetAllAsync`/`GetByIdAsync` use `AsNoTracking()` since the result only ever becomes a DTO, never gets mutated and saved back through the same instance.

Full per-controller behavior (which actions exist, what each one does) is documented in [`features.md`](features.md).

## Design Patterns in Use

### Repository Pattern

Every persisted entity has an interface in `MyTodo.Application/Repositories/Interfaces/` (e.g. `IObjectiveRepository`) and an implementation in `MyTodo.Infrastructure/Persistence/Repositories/` (e.g. `ObjectiveRepository`).

`IBaseRepository<T>` / `BaseRepository<T>` supply the common CRUD surface every entity repository inherits:

```csharp
Task<List<T>> GetAllAsync();          // AsNoTracking
Task<T?> GetByIdAsync(int id);
Task<List<T>> GetByIdsAsync(IEnumerable<int> ids);
Task AddAsync(T entity);
Task UpdateAsync(T entity);
Task UpdateRangeAsync(IEnumerable<T> entities);
Task DeleteAsync(T entity);
```

Entity-specific repositories add only the query methods particular to that entity (e.g. `IObjectiveRepository.GetBySolutionIdAsync`). Per the Application layer's own rule, **repositories never call other repositories** — any cross-entity orchestration happens one level up, in a service.

### Service Layer

Business logic lives in `MyTodo.Application/Services/*Service.cs`, never in controllers or repositories. Representative examples actually in the codebase:

- **Sort-order assignment on create** — a new Objective's `SortOrder` is set to the current count of Objectives for its Solution (append to end).
- **Timestamp stamping on status change** — `ObjectiveService`/`TodoTaskService` set `CompletedAt = DateTime.UtcNow` when status transitions *to* `Completed`, and clear it back to `null` on any transition away from it.
- **Single-invariant enforcement** — `TodoService.ToggleFrogAsync` enforces "only one Todo can be the Frog per calendar day" by un-frogging whichever other Todo held that title for the same date before frogging the new one.
- **Bulk reindexing on drag-and-drop** — `Services/Common/ReorderHelper.ReindexAsync<T>` is a shared generic routine used by every Kanban board's reorder endpoint (Solutions, Objectives, Experiments): given an ordered list of IDs, it fetches those entities in one round trip, reassigns `SortOrder` to match array position, optionally applies an extra mutation to one "anchor" entity (e.g. the card that also changed status column), and persists everything with a single `UpdateRangeAsync`.

### DTO vs. ViewModel Separation

- **DTOs** (`MyTodo.Application/DTOs/`) carry data *out of* the Application layer and are used directly as `@model` in Razor views — no parallel "display ViewModel" is created for read paths.
- **ViewModels** (`MyTodo/Models/`) carry data *into* the controller from a form or AJAX call and hold validation attributes (`[Required]`, `[StringLength]`, ...). The controller manually maps ViewModel → `Create*Dto`/`Update*Dto` before calling the service — there is no auto-mapper in this codebase by design, keeping the boundary explicit.

Full rules for this split live in [`MyTodo/CLAUDE.md`](../MyTodo/CLAUDE.md#dto-vs-viewmodel-rule).

### Dual, Independent Ordering (Kanban vs. Focus)

Solutions and Objectives each carry a single `SortOrder` column but are reordered through **two separate endpoints** (`Reorder` and `ReorderFocus`), driven by two different UI views over the same rows — the regular status-column Kanban board, and a separate "20%" high-leverage focus view filtered to `IsTwentyPercent == true`. Both endpoints reuse `ReorderHelper.ReindexAsync`, just scoped to a different subset of rows at call time.

### Dependency Injection

`MyTodo.Application` and `MyTodo.Infrastructure` each expose one static `DependencyInjection` class with an `IServiceCollection` extension method (`AddApplicationServices()`, `AddInfrastructureServices(configuration)`), keeping `Program.cs` free of a long manual list of `services.AddScoped<...>()` calls. Controllers receive everything — services and `ILogger<T>` — through constructor injection only; there is no service-locator/`IServiceProvider` usage in controllers.

## Domain Model

See [`database.md`](database.md) for the full entity-relationship diagram, table-by-table column reference, cascade-delete rules, and enum-as-string persistence strategy. Short version:

```
LifeArea ──< Problem ──< Solution ──┬──< Objective ──< TodoTask ──1:1── Todo
                                     └──< Experiment            └──> Sprint (optional)

ProblemStatusOrder — standalone lookup table (custom Kanban column order per ProblemStatus)
```

Every entity's status field (`ProblemStatus`, `SolutionStatus`, `ObjectiveStatus`, `TodoStatus`, `ExperimentStatus`) is a `MyTodo.Domain.Shared` enum, persisted as its string name rather than its numeric value.

## Cross-Cutting Concerns

- **Logging** (Serilog setup, controller logging convention) — [`logging.md`](logging.md)
- **Error handling** (`GlobalExceptionHandler`, AJAX vs. page-navigation responses) — [`error-handling.md`](error-handling.md)
- **Database schema, relationships, migration history** — [`database.md`](database.md)
- **Feature walkthrough per controller/page** — [`features.md`](features.md)
- **Local setup / running the app** — [`getting-started.md`](getting-started.md)
