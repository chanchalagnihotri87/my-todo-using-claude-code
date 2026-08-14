# Architecture

MyTodo follows Clean Architecture with four projects, each with a single, one-directional dependency:

```
MyTodo  ──depends on──▶  MyTodo.Application  ──depends on──▶  MyTodo.Domain
                                  ▲
                                  │ implements interfaces from
                                  │
                       MyTodo.Infrastructure  ──depends on──▶  MyTodo.Domain
```

- **`MyTodo.Domain`** has no dependencies on any other project. It contains only entities (`Domain/Entities`) and enums (`Domain/Enums`) — plain C# classes with no EF Core, no ASP.NET Core, no attributes.
- **`MyTodo.Application`** depends only on `MyTodo.Domain`. It defines the application's use cases as services, and declares the repository contracts it needs (`Repository/Interface`) without knowing how they're implemented.
- **`MyTodo.Infrastructure`** depends on `MyTodo.Domain` (for entities) and implements the repository interfaces declared in `MyTodo.Application`, using EF Core against SQL Server.
- **`MyTodo`** (presentation) depends only on `MyTodo.Application`. Controllers never reference `MyTodo.Infrastructure` or EF Core types directly — they only see service interfaces.

This means the Infrastructure layer could be swapped (different database, different ORM) without touching Application or Domain, and Application logic can be unit tested without spinning up a real database (by mocking repository interfaces).

## Layer Responsibilities

| Layer | Contains | Depends on |
|---|---|---|
| `MyTodo.Domain` | Entities, enums | *(nothing)* |
| `MyTodo.Application` | Services, service interfaces, DTOs, repository interfaces | `MyTodo.Domain` |
| `MyTodo.Infrastructure` | `DbContext`, EF entity configurations, migrations, repository implementations | `MyTodo.Domain`, `MyTodo.Application` (interfaces only) |
| `MyTodo` | Controllers, Razor views, input ViewModels, request/response models, middleware | `MyTodo.Application` |

## Request Flow

A typical write request (e.g. creating an Objective) flows through the layers like this:

```
Browser (jQuery $.ajax, JSON body)
   │
   ▼
Controller action (MyTodo/Controllers/ObjectivesController.cs)
   │  - validates input (ModelState / manual checks)
   │  - maps ViewModel/Request → DTO
   │  - logs Warning on invalid input / not-found
   ▼
Service (MyTodo.Application/Services/ObjectiveService.cs)
   │  - implements the service interface (IObjectiveService)
   │  - contains business logic (e.g. SortOrder assignment, CompletedAt stamping)
   │  - maps Entity → DTO for the response
   ▼
Repository (MyTodo.Infrastructure/Persistence/Repositories/ObjectiveRepository.cs)
   │  - implements the repository interface (IObjectiveRepository)
   │  - extends BaseRepository<T> for common CRUD
   │  - talks to MyTodoDbContext (EF Core)
   ▼
SQL Server (MyTodoDB_GenerateClaudeMdByOwn)
```

Read requests follow the same path but typically skip the DTO→Entity mapping step. Repository `GetAllAsync`/`GetByIdAsync` reads use `AsNoTracking()` (see `BaseRepository<T>`) since results are only ever used to build DTOs, never mutated and saved back through the same instance.

## Design Patterns in Use

### Repository Pattern
Every entity that needs persistence has:
- An interface in `MyTodo.Application/Repository/Interface/` (e.g. `IObjectiveRepository`)
- An implementation in `MyTodo.Infrastructure/Persistence/Repositories/` (e.g. `ObjectiveRepository`)

`BaseRepository<T>` (`MyTodo.Infrastructure/Persistence/Repositories/BaseRepository.cs`) implements `IBaseRepository<T>` with the common CRUD operations (`GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`) against a generic `DbSet<T>`. Entity-specific repositories inherit from it and add query methods particular to that entity (e.g. `IObjectiveRepository.GetBySolutionIdAsync`).

### Service Layer
Business logic never lives in controllers or repositories — it lives in `MyTodo.Application/Services/*Service.cs` classes. Examples of logic that belongs here (not in the controller):
- Assigning `SortOrder = existing.Count` when a new Objective is created.
- Stamping `CompletedAt` when an Objective/Task's status changes to `Completed`, and clearing it when it changes away.
- The "only one Frog per day" rule in `TodoService.ToggleFrogAsync` — un-frogging the previous frog task for that date before frogging the new one.
- Recomputing `SortOrder` for every item in a Kanban column whenever a drag-and-drop reorder happens.

### DTO vs. ViewModel Separation
- **DTOs** (`MyTodo.Application/DTOs/`) carry data *out of* the Application layer to the Controller, and are used directly as `@model` in Razor views for display — no separate display ViewModel is created.
- **ViewModels** (`MyTodo/Models/`) carry data *into* the Controller from a form or AJAX call, and hold `[Required]`/`[StringLength]` validation attributes. The controller manually maps a ViewModel to the matching `Create*Dto`/`Update*Dto` before calling the service.

This is documented in full in [`MyTodo/CLAUDE.md`](../MyTodo/CLAUDE.md).

### Dependency Injection
- `MyTodo.Application`'s services and `MyTodo.Infrastructure`'s repositories are registered via extension methods (`AddApplicationServices()`, `AddInfrastructureServices(configuration)`), called from `Program.cs`. This keeps `Program.cs` free of a long manual list of `services.AddScoped<...>()` calls.
- Controllers receive everything through constructor injection — services, and (since the logging work) `ILogger<T>`.

## Cross-Cutting Concerns

- **Logging** — see [`logging.md`](logging.md).
- **Error handling** — see [`error-handling.md`](error-handling.md).
- **Database schema & relationships** — see [`database.md`](database.md).
- **Feature walkthrough per page** — see [`features.md`](features.md).
