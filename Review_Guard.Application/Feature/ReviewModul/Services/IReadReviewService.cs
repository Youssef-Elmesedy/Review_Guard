using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Dto;

namespace Review_Guard.Application.Feature.ReviewModul.Services;

public interface IReadReviewService
{
    Task<Result<ReviewResponseDto>> GetByIdAsync(Guid reviewId, CancellationToken ct = default);
    Task<Result<PagedResult<ReviewListItemDto>>> GetMyReviewsAsync(Guid userId, PaginationParams paging, CancellationToken ct = default);
    Task<Result<PagedResult<ReviewListItemDto>>> GetPendingReviewsAsync(PaginationParams paging, CancellationToken ct = default);
}
