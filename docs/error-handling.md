# Error Handling

Unhandled exceptions — anything a controller/service/repository doesn't catch itself — are caught by a single, global handler rather than being left to each controller.

## GlobalExceptionHandler

`MyTodo/Middleware/GlobalExceptionHandler.cs` implements ASP.NET Core's `IExceptionHandler` interface (the .NET 8+ recommended way to centralize exception handling, replacing the older "catch in middleware" pattern):

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

        if (httpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
        {
            return false; // let ASP.NET Core fall through to /Home/Error
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred. Please try again." }, cancellationToken);
        return true; // handled — response already written
    }
}
```

### Registration (`Program.cs`)

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ...

app.UseExceptionHandler("/Home/Error");
```

`UseExceptionHandler("/Home/Error")` does two things when `IExceptionHandler` implementations are also registered:
1. It first gives every registered `IExceptionHandler` (here, just `GlobalExceptionHandler`) a chance to handle the exception.
2. If none of them return `true`, it falls back to re-executing the request pipeline against `/Home/Error`, which renders `Views/Home/Error.cshtml`.

This is why `GlobalExceptionHandler` always logs first, then only conditionally "handles" (returns `true`) — logging must happen exactly once regardless of which kind of response the caller ultimately gets.

Unlike the original ASP.NET Core template, this is **active in every environment**, not just non-Development — so exceptions are logged and produce a sane response locally during development too, not only in production.

## Two Response Shapes

The app has two kinds of callers, and they need different failure responses:

| Caller | Detection | Response |
|---|---|---|
| Page navigation (full page load / form POST with redirect) | No `X-Requested-With` header | Redirect to `/Home/Error`, rendering the HTML error page with a `RequestId` for correlation |
| AJAX call (jQuery `$.ajax`, used by nearly every Kanban/toggle/reorder endpoint) | `X-Requested-With: XMLHttpRequest` — set automatically by jQuery for same-origin requests | `500` response with a small JSON body `{ "error": "..." }`, so client-side `.fail()` handlers get JSON instead of an HTML error page they'd fail to parse |

## What Gets Logged

Every unhandled exception is logged exactly **once**, at `Error` level, including:
- The full exception (stack trace, inner exceptions).
- The HTTP method and path that triggered it.

This is deliberately centralized here rather than duplicated in `HomeController.Error()` — that action just renders the view; it does not re-log (see [logging.md](logging.md)).

## What This Does Not Cover

- **Expected failures** (not-found, invalid input) are not exceptions — they're handled per-action with `NotFound()`/`BadRequest()` and logged with `LogWarning` at the point of return (see [logging.md](logging.md#controller-logging-pattern)). `GlobalExceptionHandler` only ever sees genuinely unhandled exceptions (bugs, database connectivity issues, etc.).
- **Antiforgery token failures** and other ASP.NET Core pipeline-level exceptions are still caught by this handler, since it's registered globally.
- There is no retry logic, circuit breaker, or similar resilience pattern — this is purely "log it, respond gracefully" error handling, appropriate for a single-user personal application.
