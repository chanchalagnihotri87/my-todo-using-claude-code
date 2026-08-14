# Logging

The application uses **Serilog** for structured logging, replacing the default `Microsoft.Extensions.Logging` console provider.

## Setup

Wired up in `Program.cs`:

```csharp
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.File(
            path: "Logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30);
});
```

- **Sink**: `Serilog.Sinks.File`, writing to `MyTodo/Logs/log-YYYYMMDD.txt`. The file rolls over daily (`RollingInterval.Day`) and only the most recent 30 files are retained.
- **`Logs/`** is listed in `.gitignore` — log files are never committed.
- **Levels come from configuration, not code.** `ReadFrom.Configuration(context.Configuration)` reads the `Serilog` section of `appsettings.json` / `appsettings.{Environment}.json`. `Program.cs` only owns *where* logs go (the file sink); *how much* gets logged is entirely a config concern, so verbosity can be tuned per-environment without a code change or rebuild.

## Configuring Levels

`MyTodo/appsettings.json`:

```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

- `Default: "Information"` — the app's own code (controllers, services) logs at `Information` and above.
- `Override: "Microsoft.AspNetCore": "Warning"` — framework-internal logging (routing, model binding, etc.) is quieted to `Warning`+ only, so the log file isn't dominated by framework noise.

To go quieter or louder, add or edit entries here — for example, to also silence Entity Framework's own query-execution logging below `Warning`:

```json
"Override": {
  "Microsoft.AspNetCore": "Warning",
  "Microsoft.EntityFrameworkCore": "Warning"
}
```

Per-request logging (`UseSerilogRequestLogging()`) is deliberately **not** enabled — it was tried and removed, since it logs a line for every single request (including the app's many small AJAX calls) and was judged too noisy relative to the value it added. Controller-level logging (below) already captures what matters for each action.

## Controller Logging Pattern

Every controller injects `ILogger<T>` via constructor DI and follows the same two rules, established in `Controllers/TasksController.cs`:

1. **`LogWarning`** on any path that returns `NotFound()` or `BadRequest()` — i.e. when an operation didn't happen because the input or target was invalid. Include the relevant ID(s) and, for validation failures, the invalid value.
2. **`LogInformation`** on the success path of any mutation (create/update/delete/toggle/reorder) — include the ID(s) affected and, where relevant, the new value.

Example, from `LifeAreasController.Delete`:

```csharp
var deleted = await _lifeAreaService.DeleteAsync(id);
if (!deleted)
{
    _logger.LogWarning("Life area {LifeAreaId} not found when deleting", id);
    return NotFound();
}

_logger.LogInformation("Deleted life area {LifeAreaId}", id);
return RedirectToAction(nameof(Index));
```

Pure read-only, high-frequency, failure-free endpoints are deliberately **not** logged at `Information` level — see `ExploreController`, whose tree-expansion endpoints have no failure branches and are called very frequently by the UI. Logging every call there would add noise without adding diagnostic value; this mirrors the same reasoning that led to dropping per-request logging above.

Unhandled exceptions are logged separately, once, by the global exception handler — see [error-handling.md](error-handling.md) — not by individual controllers, to avoid the same exception being logged twice.

## Adding Logging to a New Controller

1. Inject `ILogger<YourController>` via the constructor, alongside your services.
2. On every `NotFound()`/`BadRequest()` return, add a `LogWarning` immediately before it with the relevant identifiers.
3. On every successful mutation, add a `LogInformation` immediately before the success `return`.
4. Don't log on pure reads unless the read itself can meaningfully fail (e.g. a not-found parent).
