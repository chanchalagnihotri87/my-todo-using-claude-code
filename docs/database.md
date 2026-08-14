# Database Schema

SQL Server database, managed entirely through EF Core migrations (`MyTodo.Infrastructure/Persistence/Migrations`). Never hand-edit that folder — use `dotnet ef migrations add <Name>`.

Connection string lives in `MyTodo/appsettings.json` under `ConnectionStrings:DefaultConnection`. Default: `Server=localhost\MSSQLSERVER01;Database=MyTodoDB_GenerateClaudeMdByOwn`.

## Entity-Relationship Overview

```
LifeArea (1) ──< (many) Problem (1) ──< (many) Solution ──┬──< (many) Objective (1) ──< (many) TodoTask (1) ──1:1── Todo
                                                            │                                        │
                                                            └──< (many) Experiment                    └──> (0..1) Sprint

ProblemStatusOrder  — standalone lookup table, one row per ProblemStatus value
```

All parent→child foreign keys shown above are `Cascade` delete **except**:
- `TodoTask.SprintId → Sprint.Id` is `SetNull` — deleting a Sprint un-assigns its tasks instead of deleting them.

## Tables

### LifeAreas
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| Name | nvarchar(200) | required |
| Description | nvarchar(1000) | nullable |
| CreatedAt | datetime2 | required |
| UpdatedAt | datetime2 | nullable |

### Problems
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| Name | nvarchar(200) | required |
| Description | nvarchar(1000) | nullable |
| Status | nvarchar(20) | enum-as-string, default `Pending` |
| IsUrgent | bit | default `false` |
| IsImportant | bit | default `false` |
| CreatedAt / UpdatedAt | datetime2 | |
| LifeAreaId | int | FK → LifeAreas, cascade delete |

`ProblemStatus`: `Pending → WorkingOnIt → Resolved` (or `Discarded`).

### ProblemStatusOrders
Lookup table storing a custom drag-reordered column order for the Problems Kanban board (see [features.md](features.md#problems)).

| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| Status | nvarchar(20) | enum-as-string, **unique index** |
| SortOrder | int | required |

### Solutions
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| Name | nvarchar(200) | required |
| Description | nvarchar(1000) | nullable |
| Status | nvarchar(20) | enum-as-string, default `Planned` |
| IsTwentyPercent | bit | default `false` — flags high-leverage ("20%") work |
| SortOrder | int | default `0` — position within its status column |
| CreatedAt / UpdatedAt | datetime2 | |
| ProblemId | int | FK → Problems, cascade delete |

`SolutionStatus`: `Planned → Verifying → Verified → AddedInRoutine → BecomeSecondNature` (or `Discarded`).

### Objectives
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| Text | nvarchar(300) | required |
| Status | nvarchar(20) | enum-as-string, default `NotStarted` |
| IsTwentyPercent | bit | default `false` |
| CompletedAt | datetime2 | nullable, set when Status → `Completed` |
| SortOrder | int | default `0` |
| CreatedAt | datetime2 | |
| SolutionId | int | FK → Solutions, cascade delete |

`ObjectiveStatus`: `NotStarted → InProgress → Completed`.

### TodoTasks
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| Name | nvarchar(200) | required |
| Status | nvarchar(20) | enum-as-string, default `Pending`, sentinel `Pending` |
| CompletedAt | datetime2 | nullable, set when Status → `Completed` |
| ObjectiveId | int | FK → Objectives, cascade delete |
| SprintId | int | nullable FK → Sprints, **set null** on Sprint delete |
| AddedInTodoList | bit | |
| TodoDate | datetime2 | |
| CreatedAt / UpdatedAt | datetime2 | |

`TodoStatus`: `Pending → InProgress → Completed`.

### Experiments
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| Name | nvarchar(200) | required |
| Description | nvarchar(1000) | nullable |
| Status | nvarchar(20) | enum-as-string, default `Innovation`, sentinel `Innovation` |
| SortOrder | int | position within its status column |
| CreatedAt / LastUpdatedAt | datetime2 | |
| SolutionId | int | FK → Solutions, cascade delete |

`ExperimentStatus`: `Innovation → Verifying → Verified → AddedInSOP` (or `Discarded`).

### Sprints
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| Name | nvarchar(200) | required |
| Description | nvarchar(1000) | nullable |
| StartDate / EndDate | datetime2 | required |
| CreatedAt / UpdatedAt | datetime2 | |

A Sprint has many `TodoTasks` (optional assignment, see above).

### Todos
A `Todo` is a `TodoTask` pulled into a specific day's working list — a strict **1:1** relationship (`Todo.TodoTaskId` is a unique FK).

| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| TodoTaskId | int | FK → TodoTasks, **unique**, cascade delete |
| TodoDate | date (DateOnly) | required |
| IsUrgent / IsImportant / IsFrog | bit | default `false` each |
| CreatedAt / UpdatedAt | datetime2 | |

Deleting the underlying `TodoTask` cascades and removes the `Todo` entry too.

## Enum-as-String Storage

Every status enum (`ProblemStatus`, `SolutionStatus`, `ObjectiveStatus`, `TodoStatus`, `ExperimentStatus`) is persisted as its **string name**, not its numeric value (`HasConversion<string>().HasMaxLength(20)`). This keeps the database self-describing and safe to reorder enum members in code without breaking existing rows — but renaming an enum member requires a data migration.

`TodoStatus` and `ExperimentStatus` also declare an explicit EF Core **sentinel value** (`HasSentinel(...)`) matching their default (`Pending` / `Innovation` respectively). This tells EF Core which value represents "not explicitly set" so it doesn't emit a misleading warning about the CLR default (`0`, which has no matching enum name since both enums start numbering at `1`).

## Migration History

Run in order (`dotnet ef database update` applies whichever are missing):

| Migration | What it added |
|---|---|
| `LifeArea_Added` | `LifeAreas` table |
| `Problem_Added` | `Problems` table |
| `Problem_LifeAreaId_ForeignKey` | FK from Problems → LifeAreas |
| `Problem_Status_Added` | `Status` column on Problems |
| `ProblemStatusOrder_Added` | `ProblemStatusOrders` lookup table |
| `Problem_UrgentImportant_Added` | `IsUrgent`, `IsImportant` on Problems |
| `Solution_Added` | `Solutions` table |
| `Objective_Added` | `Objectives` table |
| `Objective_Status_Added` | `Status` column on Objectives |
| `Objective_IsTwentyPercent_Added` | `IsTwentyPercent` on Objectives |
| `TodoTask_Added` | `TodoTasks` table |
| `TodoTask_IsCompleted_Added` | Completion tracking on TodoTasks |
| `Experiment_Added` | `Experiments` table |
| `Experiment_SortOrder_Added` | `SortOrder` on Experiments |
| `Sprint_Added` | `Sprints` table |
| `Todo_Added` | `Todos` table |
| `Todo_OneToOne_TodoTask` | Enforced 1:1 FK between Todo and TodoTask |
| `TodoTask_Status_Added_Todo_Status_Removed` | Moved status tracking from Todo to TodoTask |
| `Todo_UrgentImportantFrog_Added` | `IsUrgent`, `IsImportant`, `IsFrog` on Todos |
