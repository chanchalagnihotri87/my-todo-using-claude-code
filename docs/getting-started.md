# Getting Started

## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (a local named instance is fine — the default config targets `localhost\MSSQLSERVER01`)

## 1. Clone and configure
There's no `.sln` file — build/run commands target the `.csproj` files directly.

Check `MyTodo/appsettings.json` and update the connection string if your SQL Server instance name or database name differs:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\MSSQLSERVER01;Database=MyTodoDB_GenerateClaudeMdByOwn;Trusted_Connection=True;TrustServerCertificate=True"
}
```

## 2. Create the database
Migrations live in `MyTodo.Infrastructure/Persistence/Migrations`. Apply them from the repo root:

```bash
dotnet ef database update --project MyTodo.Infrastructure --startup-project MyTodo
```

This creates `MyTodoDB_GenerateClaudeMdByOwn` (or whatever you configured) and runs every migration listed in [database.md](database.md#migration-history).

## 3. Build and run

```bash
dotnet build
dotnet run --project MyTodo
```

Two launch profiles are defined in `MyTodo/Properties/launchSettings.json`:

| Profile | URL(s) |
|---|---|
| `https` (default) | `https://localhost:7073`, `http://localhost:5144` |
| `http` | `http://localhost:5144` |

Both set `ASPNETCORE_ENVIRONMENT=Development`. The app opens to `LifeAreas/Index` — there's no separate dashboard/landing page.

## 4. Verify it's working
- Create a Life Area, then a Problem under it, then a Solution, then an Objective, then a Task — each level's `Index` page requires its parent to exist (a `LifeAreaId`/`ProblemId`/etc. route parameter), so building the hierarchy top-down is the natural flow.
- Check `MyTodo/Logs/log-YYYYMMDD.txt` — you should see `Information`-level entries for the creates you just did (see [logging.md](logging.md)).

## Everyday Commands

| Command | Purpose |
|---|---|
| `dotnet build` | Build the whole solution (all four projects) |
| `dotnet run --project MyTodo` | Run the application |
| `dotnet ef migrations add <MigrationName>` | Add a new migration after changing an entity or its `IEntityTypeConfiguration<T>` |
| `dotnet ef database update` | Apply pending migrations |

> **Never** hand-edit files under `Migrations/` — always use the `dotnet ef` CLI. See [`MyTodo.Infrastructure/CLAUDE.md`](../MyTodo.Infrastructure/CLAUDE.md) for persistence-layer conventions.

## Where to Go Next
- [architecture.md](architecture.md) — layers, request flow, design patterns
- [database.md](database.md) — schema, relationships, migration history
- [features.md](features.md) — what every page/controller does
- [logging.md](logging.md) — Serilog setup and the controller logging pattern
- [error-handling.md](error-handling.md) — the global exception handler
