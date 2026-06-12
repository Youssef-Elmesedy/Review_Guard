using Microsoft.Extensions.Localization;
using Review_Guard.Application.Common;
using Review_Guard.Application.Common.CommonMessages;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul;
using Review_Guard.Application.Feature.ReviewModul.Dto;
using Review_Guard.Application.Feature.ReviewModul.Mapping;
using Review_Guard.Application.Feature.ReviewModul.Services;
using Review_Guard.Application.Feature.ReviewModul.Specification;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Infrastructure.Implementation.Servcices.ReviewService;

internal sealed class WriteReviewService : IWriteReviewService
{
    private readonly IReadReviewRepository  _readRepo;
    private readonly IWriteReviewRepository _writeRepo;
    private readonly IReadBranchRepository  _branchRepo;
    private readonly IWriteBranchRepository _writeBranchRepo;
    private readonly IReadBusinessRepository _businessRepo;
    private readonly INotificationService   _notifications;
    private readonly IUnitOfWork            _uow;
    private readonly ICacheService          _cache;
    private readonly ILogger<WriteReviewService> _logger;
    private readonly IStringLocalizer<WriteReviewService> _localizer;

    public WriteReviewService(
        IReadReviewRepository readRepo,
        IWriteReviewRepository writeRepo,
        IReadBranchRepository branchRepo,
        IWriteBranchRepository writeBranchRepo,
        IReadBusinessRepository businessRepo,
        INotificationService notifications,
        IUnitOfWork uow,
        ICacheService cache,
        ILogger<WriteReviewService> logger,
        IStringLocalizer<WriteReviewService> localizer)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _branchRepo = branchRepo;
        _writeBranchRepo = writeBranchRepo;
        _businessRepo = businessRepo;
        _notifications = notifications;
        _uow = uow;
        _cache = cache;
        _logger = logger;
        _localizer = localizer;
    }

    // ── Submit ────────────────────────────────────────────────────────────
    public async Task<Result<ReviewResponseDto>> SubmitAsync(
        Guid userId, SubmitReviewRequest request, CancellationToken ct = default)
    {
        try
        {
            var branch = await _branchRepo.GetByIdAsync(request.BranchId, ct);
            if (branch is null)
                return Result<ReviewResponseDto>.Failure(
                    AppErrorsCataloge.NotFound(DomainMessagies.BranchNotFound, _localizer[DomainMessagies.BranchNotFound]));

            var duplicate = await _readRepo.FindFirstAsync(
                r => r.UserId == userId && r.BranchId == request.BranchId, ct);
            if (duplicate is not null)
                return Result<ReviewResponseDto>.Failure(
                    AppErrorsCataloge.Conflict(ReviewMessage.AlreadyReviewed, _localizer[ReviewMessage.AlreadyReviewed]));

            var review = Review.Create(
                userId,
                request.BranchId,
                request.FoodRating,
                request.ServiceRating,
                request.CleanlinessRating,
                request.AmbienceRating,
                request.ValueRating,
                request.Title,
                request.Content,
                request.ProofId);

            await _writeRepo.AddAsync(review, ct);

            branch.IncrementPendingReviews();
            await _writeBranchRepo.UpdateAsync(branch, ct);

            await _uow.SaveChangesAsync(ct);

            // 🔔 Notify all admins
            await _notifications.NotifyAllAdminsAsync(
                NotificationType.NewReviewPending,
                "New review pending",
                $"A review for branch '{branch.Address}' is waiting for moderation.",
                review.Id.ToString(), "Review", ct);

            // 🔔 Notify business owner
            var business = await _businessRepo.GetByIdAsync(branch.BusinessId, ct);
            if (business is not null)
                await _notifications.NotifyBusinessOwnerAsync(
                    business.OwnerId,
                    NotificationType.NewReviewOnBranch,
                    "New review on your branch",
                    $"Someone left a review on your branch at '{branch.Address}'.",
                    review.Id.ToString(), "Review", ct);

            var dto = await _readRepo.ProjectFirstOrDefaultAsync(new ReviewByIdSpecification(review.Id), ReviewProjections.Full, ct);
            return Result<ReviewResponseDto>.Success(dto!);
        }
        catch (DomainException ex)
        {
            return Result<ReviewResponseDto>.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting review for user {UserId}", userId);
            return Result<ReviewResponseDto>.Failure(
                AppErrorsCataloge.Failure(ReviewMessage.CreateFailed, _localizer[ReviewMessage.CreateFailed]));
        }
    }

    // ── Approve ───────────────────────────────────────────────────────────
    public async Task<Result> ApproveAsync(
        Guid adminId, Guid reviewId, AdminReviewActionRequest request, CancellationToken ct = default)
    {
        try
        {
            var review = await _readRepo.GetByIdAsync(reviewId, ct);
            if (review is null)
                return Result.Failure(AppErrorsCataloge.NotFound(ReviewMessage.NotFound, _localizer[ReviewMessage.NotFound]));

            if (review.Status != ReviewStatus.Pending)
                return Result.Failure(AppErrorsCataloge.Conflict(ReviewMessage.AlreadyProcessed, _localizer[ReviewMessage.AlreadyProcessed]));

            review.Approve(adminId, request.Note);
            await _writeRepo.UpdateAsync(review, ct);

            var branch = await _branchRepo.GetByIdAsync(review.BranchId, ct);
            branch?.DecrementPendingReviews();
            if (branch is not null) await _writeBranchRepo.UpdateAsync(branch, ct);

            await _uow.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"review:{reviewId}", ct);

            // 🔔 Notify user
            await _notifications.NotifyUserAsync(
                review.UserId,
                NotificationType.ReviewApproved,
                "Your review was approved",
                $"Your review '{review.Title}' has been approved and is now live.",
                review.Id.ToString(), "Review", ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving review {ReviewId}", reviewId);
            return Result.Failure(AppErrorsCataloge.Failure(ReviewMessage.ApproveFailed, _localizer[ReviewMessage.ApproveFailed]));
        }
    }

    // ── Reject ────────────────────────────────────────────────────────────
    public async Task<Result> RejectAsync(
        Guid adminId, Guid reviewId, AdminReviewActionRequest request, CancellationToken ct = default)
    {
        try
        {
            var review = await _readRepo.GetByIdAsync(reviewId, ct);
            if (review is null)
                return Result.Failure(AppErrorsCataloge.NotFound(ReviewMessage.NotFound, _localizer[ReviewMessage.NotFound]));

            if (review.Status != ReviewStatus.Pending)
                return Result.Failure(AppErrorsCataloge.Conflict(ReviewMessage.AlreadyProcessed, _localizer[ReviewMessage.AlreadyProcessed]));

            var reason = request.Note ?? string.Empty;
            review.Reject(adminId, reason);
            await _writeRepo.UpdateAsync(review, ct);

            var branch = await _branchRepo.GetByIdAsync(review.BranchId, ct);
            branch?.DecrementPendingReviews();
            if (branch is not null) await _writeBranchRepo.UpdateAsync(branch, ct);

            await _uow.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"review:{reviewId}", ct);

            // 🔔 Notify user
            await _notifications.NotifyUserAsync(
                review.UserId,
                NotificationType.ReviewRejected,
                "Your review was rejected",
                $"Your review '{review.Title}' was rejected. Reason: {reason}",
                review.Id.ToString(), "Review", ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting review {ReviewId}", reviewId);
            return Result.Failure(AppErrorsCataloge.Failure(ReviewMessage.RejectFailed, _localizer[ReviewMessage.RejectFailed]));
        }
    }

    // ── Delete ────────────────────────────────────────────────────────────
    public async Task<Result> DeleteAsync(
        Guid callerId, bool isAdmin, Guid reviewId, CancellationToken ct = default)
    {
        try
        {
            var review = await _readRepo.GetByIdAsync(reviewId, ct);
            if (review is null)
                return Result.Failure(AppErrorsCataloge.NotFound(ReviewMessage.NotFound, _localizer[ReviewMessage.NotFound]));

            if (!isAdmin && review.UserId != callerId)
                return Result.Failure(AppErrorsCataloge.Forbidden(ReviewMessage.Forbidden, _localizer[ReviewMessage.Forbidden]));

            await _writeRepo.DeleteAsync(review, ct);
            await _uow.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"review:{reviewId}", ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review {ReviewId}", reviewId);
            return Result.Failure(AppErrorsCataloge.Failure(ReviewMessage.DeleteFailed, _localizer[ReviewMessage.DeleteFailed]));
        }
    }
}
