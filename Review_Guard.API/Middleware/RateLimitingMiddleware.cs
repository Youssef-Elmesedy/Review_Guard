using System.Collections.Concurrent;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace Review_Guard.API.Middleware;

/// <summary>
/// Sliding-window rate limiter.
/// Limits requests per authenticated user-ID (preferred) or client IP.
/// Configure via appsettings: "RateLimiting": { "RequestsPerWindow": 60, "WindowSeconds": 60 }
/// </summary>
public sealed class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;

    // key → timestamps of requests inside the sliding window
    private static readonly ConcurrentDictionary<string, Queue<DateTime>> _windows = new();

    // Periodic cleanup to prevent memory growth
    private static readonly Timer _cleanupTimer = new(
        _ => Cleanup(), null,
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(5));

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _options = configuration.GetSection("RateLimiting").Get<RateLimitOptions>()
                   ?? new RateLimitOptions();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip health-checks, swagger and static assets
        if (ShouldBypass(context))
        {
            await _next(context);
            return;
        }

        var key = BuildKey(context);
        var now = DateTime.UtcNow;
        var windowEdge = now.AddSeconds(-_options.WindowSeconds);

        var window = _windows.GetOrAdd(key, _ => new Queue<DateTime>());
        bool denied;
        int retryAfterSeconds;

        lock (window)
        {
            // evict stale timestamps
            while (window.Count > 0 && window.Peek() < windowEdge)
                window.Dequeue();

            denied = window.Count >= _options.RequestsPerWindow;

            if (denied)
            {
                retryAfterSeconds = (int)Math.Ceiling((window.Peek() - windowEdge).TotalSeconds);
            }
            else
            {
                window.Enqueue(now);
                retryAfterSeconds = 0;
            }
        }

        if (denied)
        {
            _logger.LogWarning(
                "Rate limit hit | Key={Key} | Path={Path}",
                key, context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";
            context.Response.Headers["Retry-After"] = retryAfterSeconds.ToString();
            context.Response.Headers["X-RateLimit-Limit"] = _options.RequestsPerWindow.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = "0";

            var body = JsonSerializer.Serialize(new
            {
                success = false,
                errorCode = "RateLimit.Exceeded",
                message = $"Too many requests. Retry after {retryAfterSeconds}s.",
                retryAfterSeconds,
                timestamp = now
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(body);
            return;
        }

        // Attach informational headers to successful responses
        context.Response.OnStarting(() =>
        {
            lock (window)
            {
                var remaining = Math.Max(0, _options.RequestsPerWindow - window.Count);
                if (!context.Response.Headers.ContainsKey("X-RateLimit-Limit"))
                {
                    context.Response.Headers["X-RateLimit-Limit"] = _options.RequestsPerWindow.ToString();
                    context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
                    context.Response.Headers["X-RateLimit-Window"] = _options.WindowSeconds.ToString();
                }
            }
            return Task.CompletedTask;
        });

        await _next(context);
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static string BuildKey(HttpContext ctx)
    {
        var userId = ctx.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
            return $"uid:{userId}";

        var ip = ctx.Request.Headers["X-Forwarded-For"]
                     .FirstOrDefault()
                     ?.Split(',')[0].Trim()
                 ?? ctx.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";

        return $"ip:{ip}";
    }

    private static bool ShouldBypass(HttpContext ctx)
    {
        var path = ctx.Request.Path.Value ?? string.Empty;
        return path.StartsWith("/health", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase);
    }

    private static void Cleanup()
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-10);
        foreach (var key in _windows.Keys.ToList())
        {
            if (_windows.TryGetValue(key, out var w))
            {
                lock (w)
                {
                    if (w.Count == 0 || w.Peek() < cutoff)
                        _windows.TryRemove(key, out _);
                }
            }
        }
    }
}

/// <summary>Bound from appsettings "RateLimiting" section.</summary>
public sealed class RateLimitOptions
{
    public int RequestsPerWindow { get; set; } = 25;
    public int WindowSeconds { get; set; } = 60;
}
