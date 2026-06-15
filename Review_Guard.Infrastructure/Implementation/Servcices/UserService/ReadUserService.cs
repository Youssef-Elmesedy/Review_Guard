namespace Review_Guard.Infrastructure.Implementation.Servcices.UserService;

internal sealed class ReadUserService : IReadUserService
{
    private readonly IReadUserRepository _readUserRepo;
    private readonly IReadUserActivityRepository _activityRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;
    private readonly ILogger<ReadUserService> _logger;
    private readonly IStringLocalizer<ReadUserService> _localizer;

    public ReadUserService(
        IReadUserRepository readUserRepo,
        IReadUserActivityRepository activityRepo,
        ICurrentUserService currentUser,
        ICacheService cache,
        ILogger<ReadUserService> logger,
        IStringLocalizer<ReadUserService> localizer)
    {
        _readUserRepo = readUserRepo;
        _activityRepo = activityRepo;
        _currentUser = currentUser;
        _cache = cache;
        _logger = logger;
        _localizer = localizer;
    }

    // ── GetProfile ────────────────────────────────────────────────────────
    public async Task<Result<UserProfileResponse>> GetProfileAsync(
        Guid userId, CancellationToken ct = default)
    {
        var cacheKey = $"user:profile:{userId}";
        try
        {
            var cached = await _cache.GetAsync<UserProfileResponse>(cacheKey, ct);
            if (cached is not null)
                return Result<UserProfileResponse>.Success(cached);

            if (_currentUser.UserId != userId)
                return Result<UserProfileResponse>.Failure(
                    AppErrorsCataloge.Forbidden(
                        _localizer[AuthMessage.InvalidCredentials],
                        _localizer[AuthMessage.InvalidCredentials]));

            var user = await _readUserRepo.ProjectFirstOrDefaultAsync(
                new GetUserProfileSpecification(userId),
                GetUserProjection.UserProfile, ct);

            if (user is null)
                return Result<UserProfileResponse>.Failure(
                    AppErrorsCataloge.NotFound(
                        UserMessage.UserNotFound,
                        _localizer[UserMessage.UserNotFound]));

            var stats = await _readUserRepo.GetUserReviewStatsAsync(userId, ct);

            var result = user with
            {
                TotalReviews = stats.TotalReviews,
                AverageRating = stats.AverageRating,
                ApprovedReviews = stats.ApprovedReviews,
                RejectedReviews = stats.RejectedReviews
            };

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30), ct);

            return Result<UserProfileResponse>.Success(result);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error fetching profile {UserId}", userId);
            return Result<UserProfileResponse>.Failure(
                AppErrorsCataloge.Failure(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching profile {UserId}", userId);
            return Result<UserProfileResponse>.Failure(
                AppErrorsCataloge.Failure("UnexpectedError", _localizer["UnexpectedError"]));
        }
    }

    // ── GetAllUsers (Admin) ────────────────────────────────────────────────
    public async Task<Result<PagedResult<UserListItemDto>>> GetAllUsersAsync(
        PaginationParams paging, CancellationToken ct = default)
    {
        var cacheKey = $"user:list:{paging.PageNumber}:{paging.PageSize}";
        try
        {
            var cached = await _cache.GetAsync<PagedResult<UserListItemDto>>(cacheKey, ct);
            if (cached is not null)
                return Result<PagedResult<UserListItemDto>>.Success(cached);

            var spec = new GetAllUsersSpecification(paging);

            var items = await _readUserRepo.ProjectAsync(spec, GetUserProjection.UserListItem, ct);

            var total = await _readUserRepo.CountAsync(u => u.Role == Roles.User, ct);

            var response = PagedResult<UserListItemDto>.Create(items, total, paging.PageNumber, paging.PageSize);

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), ct);

            return Result<PagedResult<UserListItemDto>>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all users");
            return Result<PagedResult<UserListItemDto>>.Failure(
                AppErrorsCataloge.Failure(UserMessage.GetAllUsersFailed, _localizer[UserMessage.GetAllUsersFailed]));
        }
    }

    // ── GetUserActivities ─────────────────────────────────────────────────
    public async Task<Result<PagedResult<UserActivityDto>>> GetUserActivitiesAsync(
        Guid userId, PaginationParams paging, CancellationToken ct = default)
    {
        try
        {
            var spec = new GetUserActivitiesSpecification(userId, paging);

            var items = await _activityRepo.ProjectAsync(spec, GetUserProjection.Activity, ct);

            var total = await _activityRepo.CountAsync(a => a.UserId == userId, ct);

            return Result<PagedResult<UserActivityDto>>.Success(
                PagedResult<UserActivityDto>.Create(items, total, paging.PageNumber, paging.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching activities for {UserId}", userId);
            return Result<PagedResult<UserActivityDto>>.Failure(
                AppErrorsCataloge.Failure(UserMessage.GetUserActivitiesFailed, _localizer[UserMessage.GetUserActivitiesFailed]));
        }
    }
}
