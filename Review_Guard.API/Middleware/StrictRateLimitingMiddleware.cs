using System.Collections.Concurrent;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace Review_Guard.API.Middleware;

/// <summary>
/// Fixed-window per-second throttle: rejects any client (same authenticated user,
/// or same IP if anonymous) that sends more than <see cref="StrictRateLimitOptions.RequestsPerSecond"/>
/// requests within a single second.
///
/// This runs IN ADDITION to <see cref="RateLimitingMiddleware"/> (per-minute sliding window).
/// It exists specifically to stop bursts/abuse: e.g. a script firing 50 requests instantly.
///
/// Configure via appsettings: "StrictRateLimiting": { "RequestsPerSecond": 10 }
/// </summary>
public sealed class StrictRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<StrictRateLimitingMiddleware> _logger;
    private readonly StrictRateLimitOptions _options;

    // key → (second bucket, count within that second)
    private static readonly ConcurrentDictionary<string, Bucket> _buckets = new();

    private static readonly Timer _cleanupTimer = new(
        _ => Cleanup(), null,
        TimeSpan.FromMinutes(2),
        TimeSpan.FromMinutes(2));

    public StrictRateLimitingMiddleware(
        RequestDelegate next,
        ILogger<StrictRateLimitingMiddleware> logger,
        IConfiguration configuration)
    {
        _next    = next;
        _logger  = logger;
        _options = configuration.GetSection("StrictRateLimiting").Get<StrictRateLimitOptions>()
                   ?? new StrictRateLimitOptions();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldBypass(context))
        {
            await _next(context);
            return;
        }

        var key           = BuildKey(context);
        var currentSecond = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;

        var bucket = _buckets.GetOrAdd(key, _ => new Bucket());

        int countThisSecond;
        lock (bucket)
        {
            if (bucket.Second != currentSecond)
            {
                bucket.Second = currentSecond;
                bucket.Count  = 0;
            }

            bucket.Count++;
            countThisSecond = bucket.Count;
        }

        if (countThisSecond > _options.RequestsPerSecond)
        {
            _logger.LogWarning(
                "Per-second rate limit exceeded | Key={Key} | Path={Path} | Count={Count}",
                key, context.Request.Path, countThisSecond);

            context.Response.StatusCode  = (int)HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";
            context.Response.Headers["Retry-After"] = "1";

            var body = JsonSerializer.Serialize(new
            {
                success   = false,
                errorCode = "RateLimit.TooManyRequestsPerSecond",
                message   = $"Too many requests. Max {_options.RequestsPerSecond} requests/second per client.",
                retryAfterSeconds = 1
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(body);
            return;
        }

        await _next(context);
    }

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
        return path.StartsWith("/health",  StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase);
    }

    private static void Cleanup()
    {
        var cutoffSecond = (DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond) - 5;
        foreach (var kvp in _buckets)
        {
            if (kvp.Value.Second < cutoffSecond)
                _buckets.TryRemove(kvp.Key, out _);
        }
    }

    private sealed class Bucket
    {
        public long Second;
        public int  Count;
    }
}

/// <summary>Bound from appsettings "StrictRateLimiting" section.</summary>
public sealed class StrictRateLimitOptions
{
    /// <summary>Maximum requests allowed from the same client within one second. Default: 10.</summary>
    public int RequestsPerSecond { get; set; } = 10;
}
