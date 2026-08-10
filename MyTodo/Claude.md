# MyTodo — Notes (Presentation Layer)

## Folder layout
- `Controllers/` – MVC controllers, thin — no business logic here
- `Views/` – Razor views, organized by controller name
- `Models/` – ViewModels only (input/form models), not domain entities
- `wwwroot/` – static files (css, js, images)

## DTO vs ViewModel rule
- **Display (Controller → View):** use the DTO from `MyTodo.Application.DTOs` directly as `@model`. No separate ViewModel needed.
- **Input (View → Controller, forms):** use a ViewModel in `Models/` for binding + validation attributes (`[Required]`, `[StringLength]`, etc.), plus any UI-only data (dropdown lists, checkboxes).
- Map ViewModel → DTO/Command manually in the controller before calling the service.

## Rules
- Controllers depend only on `Service/Interface/` from `MyTodo.Application` (e.g. `ITaskService`) — never call repositories or DbContext directly.
- Views must NOT call services directly (no `@inject ITaskService` for data-fetching) — all data fetching happens in the Controller. Views only display what's passed to them.
- Create partial views for reusable UI components (e.g. `_TaskListPartial.cshtml`), and include them in other views via `@Html.Partial("_TaskListPartial", Model.Tasks)`. Create partial views to keep page views clean and DRY.
- Keep controller actions short: validate input → map to DTO → call service → return view/result.
- Use ` validation attributes on ViewModels — don't duplicate validation logic already in Application layer.
- Create separate jquery/JS files for each view that needs JS, and include them in the view via `<script src="..."></script>`.
- Create separate CSS files for each view that needs custom styling, and include them in the view via `<link rel="stylesheet" href="...">`.

## Naming
- ViewModels (input only) suffixed with `ViewModel` (e.g. `CreateTaskViewModel`)
- Controller names match resource plural (e.g. `TasksController`)

## Don'ts
- Don't put EF Core or SQL logic in controllers or views.
- Don't reference `MyTodo.Infrastructure` directly from this project — always go through `MyTodo.Application` interfaces.
- Don't create a ViewModel just to mirror a DTO for display — use the DTO directly.
- Don't create a ViewModel just to mirror a DTO for display — use the DTO directly.