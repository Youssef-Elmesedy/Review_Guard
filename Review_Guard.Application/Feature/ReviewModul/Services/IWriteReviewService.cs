using Review_Guard.Application.Feature.ReviewModul.Dto;

namespace Review_Guard.Application.Feature.ReviewModul.Services;

public interface IWriteReviewService
{
    Task<Result<ReviewResponseDto>> SubmitAsync(Guid userId, SubmitReviewRequest request, CancellationToken ct = default);
    Task<Result> ApproveAsync(Guid adminId, Guid reviewId, AdminReviewActionRequest request, CancellationToken ct = default);
    Task<Result> RejectAsync(Guid adminId, Guid reviewId, AdminReviewActionRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid callerId, bool isAdmin, Guid reviewId, CancellationToken ct = default);
}
