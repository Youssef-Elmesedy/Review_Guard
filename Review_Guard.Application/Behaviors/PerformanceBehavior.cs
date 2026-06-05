using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Review_Guard.Application.Behaviors;

/// <summary>
/// Logs a warning when a request takes longer than the threshold (default 1000ms).
/// </summary>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private const int WarningThresholdMs = 1000;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        if (sw.ElapsedMilliseconds > WarningThresholdMs)
            _logger.LogWarning("[PERFORMANCE] {RequestName} exceeded threshold: {Elapsed}ms (threshold: {Threshold}ms)",
                typeof(TRequest).Name, sw.ElapsedMilliseconds, WarningThresholdMs);

        return response;
    }
}
