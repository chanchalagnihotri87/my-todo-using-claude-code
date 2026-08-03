# MyTodo.Application - Notes

## Folder Layout
- `Repository/Interface`: Contains repository interface definitions for data access.
- `Services`: Contains service classes that implement business logic and interact with repositories.
- `Services/Interfaces`: Contains service interface definitions for dependency injection and abstraction.
-  `DTOs`: Contains Data Transfer Objects used for transferring data between layers.

## Rules
- Every repository gets an interface first in `Repository/Interface/`, implementation lives in `MyTodo.Infrastructure/Persistence/Repositories/`.
- Every service gets an interface in `Service/Interface/` before the concrete class in `Service/`.
- Controllers depend only on interfaces (`ITaskService`, not `TaskService`) — enables DI and testing.
- Services orchestrate repositories; repositories never call other repositories directly.


## SOLID & Clean Code
- **Single Responsibility** – one service/class does one thing (e.g. `TaskService` handles tasks only, not reminders).
- **Open/Closed** – extend via new classes/interfaces, avoid editing existing working logic unless necessary.
- **Liskov Substitution** – implementations must be fully swappable via their interface, no surprise behavior.
- **Interface Segregation** – keep interfaces small and focused (don't dump unrelated methods into one interface).
- **Dependency Inversion** – depend on interfaces (`ITaskRepository`), never concrete classes.
- Keep methods short and named for what they do; avoid deep nesting — prefer early returns.
- No business logic in controllers or repositories — it belongs in `Service/`.


## Naming
- Interfaces prefixed with `I` (e.g. `ITaskService`)
- DTOs suffixed with `Dto` (e.g. `TaskDto`, `CreateTaskDto`)