using Review_Guard.Application.Feature.ReviewModul;
using Review_Guard.Application.Feature.ReviewModul.Dto;
using Review_Guard.Application.Feature.ReviewModul.Mapping;
using Review_Guard.Application.Feature.ReviewModul.Services;
using Review_Guard.Application.Feature.ReviewModul.Specification;

namespace Review_Guard.Infrastructure.Implementation.Servcices.ReviewService;

internal sealed class ReadReviewService : IReadReviewService
{
    private readonly IReadReviewRepository _repo;
    private readonly ICacheService _cache;
    private readonly ILogger<ReadReviewService> _logger;
    private readonly IStringLocalizer<ReadReviewService> _localizer;

    public ReadReviewService(
        IReadReviewRepository repo,
        ICacheService cache,
        ILogger<ReadReviewService> logger,
        IStringLocalizer<ReadReviewService> localizer)
    {
        _repo = repo;
        _cache = cache;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<Result<ReviewResponseDto>> GetByIdAsync(Guid reviewId, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = $"review:{reviewId}";
            var cached = await _cache.GetAsync<ReviewResponseDto>(cacheKey, ct);
            if (cached is not null)
                return Result<ReviewResponseDto>.Success(cached);

            var dto = await _repo.ProjectFirstOrDefaultAsync(new ReviewByIdSpecification(reviewId), ReviewProjections.Full, ct);
            if (dto is null)
                return Result<ReviewResponseDto>.Failure(
                    AppErrorsCataloge.NotFound(_localizer[ReviewMessage.NotFound]));

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10), ct);
            return Result<ReviewResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching review {ReviewId}", reviewId);
            return Result<ReviewResponseDto>.Failure(
                AppErrorsCataloge.Failure(_localizer[ReviewMessage.FetchFailed]));
        }
    }

    public async Task<Result<PagedResult<ReviewListItemDto>>> GetMyReviewsAsync(
        Guid userId, PaginationParams paging, CancellationToken ct = default)
    {
        try
        {
            var items = await _repo.ProjectAsync(new MyReviewsSpecification(userId, paging), ReviewProjections.ListItem, ct);
            var total = await _repo.CountAsync(r => r.UserId == userId, ct);

            return Result<PagedResult<ReviewListItemDto>>.Success(
                PagedResult<ReviewListItemDto>.Create(items, total, paging.PageNumber, paging.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching reviews for user {UserId}", userId);
            return Result<PagedResult<ReviewListItemDto>>.Failure(
                AppErrorsCataloge.Failure(_localizer[ReviewMessage.FetchFailed]));
        }
    }

    public async Task<Result<PagedResult<ReviewListItemDto>>> GetPendingReviewsAsync(
        PaginationParams paging, CancellationToken ct = default)
    {
        try
        {
            var items = await _repo.ProjectAsync(new PendingReviewsSpecification(paging), ReviewProjections.ListItem, ct);
            var total = await _repo.CountAsync(r => r.Status == ReviewStatus.Pending, ct);

            return Result<PagedResult<ReviewListItemDto>>.Success(
                PagedResult<ReviewListItemDto>.Create(items, total, paging.PageNumber, paging.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pending reviews");
            return Result<PagedResult<ReviewListItemDto>>.Failure(
                AppErrorsCataloge.Failure(_localizer[ReviewMessage.FetchFailed]));
        }
    }
}
