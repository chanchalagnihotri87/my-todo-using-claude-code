# Features

This page walks through every page/controller and the behavior it implements. Routes follow the default MVC pattern `{controller}/{action}/{id?}`, with `LifeAreas/Index` as the app's home route.

## Life Areas

**Controller:** `LifeAreasController` · **Views:** `Views/LifeAreas/`

The top level of the hierarchy — broad categories of life (e.g. "Health", "Career"). Full page CRUD (not AJAX):
- `Index` — lists all Life Areas.
- `Create` (GET renders form, POST submits) — server-rendered form with `ModelState` validation via `CreateLifeAreaViewModel`.
- `Edit` (GET renders form, POST submits) — `EditLifeAreaViewModel`.
- `Delete` (POST) — cascades to delete all Problems (and everything beneath them) under that Life Area.

## Problems

**Controller:** `ProblemsController` · **Views:** `Views/Problems/`

Problems belong to a Life Area. The Problems page is a **Kanban board** with drag-and-drop:
- `Index(lifeAreaId)` — loads the board plus the custom column order (`IProblemStatusOrderService.GetOrderAsync`).
- `Create` — GET renders a form scoped to a Life Area; POST creates via `CreateProblemDto`.
- `Edit` (AJAX, `[FromBody]`) — updates name/description/status/urgent/important via a modal.
- `UpdateStatus` (AJAX) — dedicated endpoint for a drag between Kanban columns (status change only).
- `ReorderLists` (AJAX) — persists a custom **column order** for the board itself (not the cards) into `ProblemStatusOrders`. Any `ProblemStatus` not yet in that table is appended at the end, so new enum values automatically show up.
- `ToggleUrgent` / `ToggleImportant` (AJAX) — Eisenhower-matrix tagging, returns just the new boolean so the UI can flip a badge without a full reload.
- `Delete` (AJAX) — cascades to delete all Solutions beneath the Problem.

## Solutions

**Controller:** `SolutionsController` · **Views:** `Views/Solutions/`

Solutions belong to a Problem, and are the branching point for both **Objectives** (planned work) and **Experiments** (unproven ideas to validate).
- `Index(problemId)` — Kanban board of Solutions for a Problem.
- `Details(id)` — a Solution's detail page, which hosts the tabs for its Objectives and Experiments (see `_SolutionTabHeader.cshtml`).
- `Create` — GET/POST form, scoped to a Problem.
- `Reorder` (AJAX) — drag-and-drop within/between status columns; recomputes `SortOrder` for every card in the target column (see [architecture.md](architecture.md#service-layer)).
- `ReorderFocus` (AJAX) — a **second, independent ordering** used by the "20%" focus view (`IsTwentyPercent` flag + its own `SortOrder` pass), letting the same Solutions be ranked differently in the regular Kanban board vs. the focus board.

## Objectives

**Controller:** `ObjectivesController` · **Views:** `Views/Objectives/`

Objectives are concrete goals under a Solution, and are the parent of Tasks.
- `Index(solutionId)` — Kanban board (`NotStarted` / `InProgress` / `Completed`).
- `Create` (AJAX) — `SortOrder` is set to the current count of Objectives for that Solution (append to end).
- `Edit` (AJAX) — updates text/status; setting `Status = Completed` stamps `CompletedAt = UtcNow`, and clearing away from `Completed` clears it back to `null`.
- `Reorder` (AJAX) — status-column drag, same recompute-all-`SortOrder` pattern as Solutions.
- `ReorderFocus` (AJAX) — independent "20%" focus ordering, same pattern as Solutions.
- `Delete` (AJAX) — cascades to delete all Tasks beneath the Objective.

## Tasks

**Controller:** `TasksController` · **Views:** `Views/Tasks/`

Tasks are the actionable, schedulable unit under an Objective — the level that connects to Sprints and to the daily Todo list.
- `Index(objectiveId)` — lists Tasks for an Objective, alongside the list of Sprints (for the assignment dropdown).
- `Create` (AJAX) — simple create under an Objective.
- `Edit` (AJAX) — updates name/status/sprint assignment together. Status changes to/from `Completed` stamp/clear `CompletedAt` (`TodoTaskService.SetStatus`).
- `UpdateStatus` (AJAX) — status-only update, used for quick checkbox-style toggles.
- `UpdateSprint` (AJAX) — assigns/unassigns a Task to a Sprint independently of status.
- `AddToTodo` (AJAX) — pulls the Task into today's Todo list (`ITodoService.AddToTodoAsync`); idempotent — calling it again for a Task that's already on today's list returns the existing `Todo` instead of creating a duplicate.
- `UpdateTodoDate` (AJAX) — reschedules the Task's Todo entry to a different day.
- `Delete` (AJAX) — deletes the Task; its `Todo` entry (if any) cascades with it.

This is also the controller documented in most detail in code, since it was the first to receive full request logging — see `Controllers/TasksController.cs` for the canonical `ILogger<T>` usage pattern followed by every other controller.

## Experiments

**Controller:** `ExperimentsController` · **Views:** `Views/Experiments/`

The second branch under a Solution — for validating unproven ideas before committing to them as an Objective/routine.
- `Index(solutionId)` — Kanban board (`Innovation → Verifying → Verified → AddedInSOP`, or `Discarded`).
- `Create` / `Edit` / `Reorder` / `Delete` (all AJAX) — same shape as Objectives' Kanban endpoints, using `SortOrder` recompute on drag.

## Sprints

**Controller:** `SprintsController` · **Views:** `Views/Sprints/`

A Sprint is a time-boxed period (`StartDate`–`EndDate`) that Tasks can be pulled into, independent of which Objective they belong to — useful for planning "what am I working on this week" across multiple Objectives at once.
- `Index` — shows the *current* Sprint (`ISprintService.GetCurrentAsync`, presumably the one whose date range includes today) and its Tasks; renders an empty state if there is no current Sprint.
- `All` — lists every Sprint, past and future.
- `Details(id)` — a specific Sprint's Tasks.
- `Create` / `Edit` / `Delete` (AJAX) — standard CRUD via modals.

## Todos

**Controller:** `TodosController` · **Views:** `Views/Todos/`

The daily execution view — Tasks that have been explicitly pulled in via `TasksController.AddToTodo`.
- `Index` — today's Todo list (`ITodoService.GetTodayAsync`).
- `History(objectiveId?, date?, week?)` — a filterable history view. Accepts either a specific `date`, or an ISO week string (`"2026-W07"` format, parsed by `TodosController.TryParseWeek`) to filter to a Monday–Sunday range, optionally further filtered by Objective.
- `ToggleUrgent` / `ToggleImportant` (AJAX) — Eisenhower tagging on the Todo entry itself (distinct from the same tags on Problems).
- `ToggleFrog` (AJAX) — "Eat That Frog" prioritization: marks a Todo as *the* one hardest/most important task for that day. **Only one Todo can be the Frog per calendar day** — `TodoService.ToggleFrogAsync` automatically un-frogs whichever other Todo held that title for the same `TodoDate` before frogging the new one.

## Explore

**Controller:** `ExploreController` · **View:** `Views/Explore/Index.cshtml`

A read-only, lazily-expanding tree view of the entire hierarchy, used to browse everything (Life Area → Problem → Solution → Objective → Task) in one place without navigating page-to-page. Each level is its own AJAX `[HttpGet]` action returning a partial view (`_ProblemNodes`, `_SolutionNodes`, `_ObjectiveNodes`, `_TaskNodes`) so a branch only loads its children when the user expands it in the UI.

## Home

**Controller:** `HomeController`

Just the `Error` action — the page unhandled exceptions land on (see [error-handling.md](error-handling.md)). There is no dashboard/landing page; the app's default route is `LifeAreas/Index`.
