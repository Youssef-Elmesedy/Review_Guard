using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Review_Guard.Application.Abstractions.Services.CurrentUserService;

namespace Review_Guard.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that transparently caches query results using
/// <see cref="IMemoryCache"/>. Only requests that implement <see cref="ICacheable"/>
/// are cached; all others pass straight through.
///
/// User-scoped queries (IsUserScoped = true) automatically include the current
/// user's ID in the cache key to prevent cross-user data leakage.
/// </summary>
public sealed class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMemoryCache _cache;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(
        IMemoryCache cache,
        ICurrentUserService currentUser,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (request is not ICacheable cacheable)
            return await next();

        // Build the effective cache key — append userId for user-scoped queries
        var key = cacheable.CacheKey;
        if (cacheable.IsUserScoped && _currentUser.UserId.HasValue)
            key = $"{key}:user:{_currentUser.UserId.Value}";

        if (_cache.TryGetValue(key, out TResponse? cached) && cached is not null)
        {
            _logger.LogDebug("[Cache HIT]  {Key}", key);
            return cached;
        }

        _logger.LogDebug("[Cache MISS] {Key}", key);
        var result = await next();

        _cache.Set(key, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheable.CacheDuration,
            SlidingExpiration = null
        });

        return result;
    }
}