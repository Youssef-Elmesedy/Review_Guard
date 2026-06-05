namespace Review_Guard.Application.Behaviors;

/// <summary>
/// Marker interface — implement on a query record to opt into IMemoryCache caching.
/// </summary>
public interface ICacheable
{
    string CacheKey { get; }
    TimeSpan CacheDuration => TimeSpan.FromMinutes(5);

    /// <summary>
    /// When true the CachingBehavior appends ":user:{userId}" to the key so
    /// each user gets an isolated cache entry, preventing cross-user data leakage.
    /// </summary>
    bool IsUserScoped => false;
}
