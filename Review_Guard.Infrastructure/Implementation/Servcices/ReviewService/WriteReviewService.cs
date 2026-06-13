using Microsoft.Extensions.Localization;
using Review_Guard.Application.Common;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul;
using Review_Guard.Application.Feature.ReviewModul;
using Review_Guard.Application.Feature.ReviewModul.Dto;
using Review_Guard.Application.Feature.ReviewModul.Mapping;
using Review_Guard.Application.Feature.ReviewModul.Services;
using Review_Guard.Application.Feature.ReviewModul.Specification;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Infrastructure.Implementation.Servcices.ReviewService;

internal sealed class WriteReviewService : IWriteReviewService
{
    private readonly IReadReviewRepository _readRepo;
    private readonly IWriteReviewRepository _writeRepo;
    private readonly IReadBranchRepository _branchRepo;
    private readonly IWriteBranchRepository _writeBranchRepo;
    private readonly IReadBusinessRepository _businessRepo;
    private readonly IReadProofRepository _proofRepository;
    private readonly INotificationService _notifications;
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;
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
        IStringLocalizer<WriteReviewService> localizer,
        IReadProofRepository proofRepository)
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
        _proofRepository = proofRepository;
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

            var duplicate = await _readRepo.AnyAsync(
                r => r.UserId == userId && r.BranchId == request.BranchId, ct);
            if (duplicate)
                return Result<ReviewResponseDto>.Failure(
                    AppErrorsCataloge.Conflict(ReviewMessage.AlreadyReviewed, _localizer[ReviewMessage.AlreadyReviewed]));
            /*
            // ── Business Rules ─────────────────────────────────────────────────
            var eligibility = ReviewBusinessRules.UserMustBeEligibleToReview();
            if (eligibility.IsFailure)
                return Result.Failure<AddReviewResponse>(
                    eligibility.ErrorKey,
                    eligibility.ErrorArguments);

            var ownBiz = ReviewBusinessRules.UserCannotReviewOwnBusiness(user, business);
            if (ownBiz.IsFailure)
                return Result.Failure<AddReviewResponse>(
                    ownBiz.ErrorKey,
                    ownBiz.ErrorArguments);

            var alreadyReviewed = await _reviewRepo
                .UserHasReviewedBusinessAsync(userId, businessId, ct);
            var dupCheck = ReviewBusinessRules.UserHasNotAlreadyReviewedBusiness(alreadyReviewed);
            if (dupCheck.IsFailure)
                return Result.Failure<AddReviewResponse>(
                    dupCheck.ErrorKey,
                    dupCheck.ErrorArguments);

            // ── Proof Validation ───────────────────────────────────────────────
            Proof? proof = null;
            if (proofId.HasValue)
            {
                proof = await _proofRepo.GetByIdAsync(proofId.Value, ct);
                if (proof is null) return Result.Failure<AddReviewResponse>("Proof not found.");
            }

            var proofRule = ReviewBusinessRules.ProofRequiredForUserLevel(user, proof);
            if (proofRule.IsFailure)
                return Result.Failure<AddReviewResponse>(
                    proofRule.ErrorKey,
                    proofRule.ErrorArguments);

            if (proof is not null)
            {
                var proofOwner = ReviewBusinessRules.ProofMustBelongToUser(proof, userId);
                if (proofOwner.IsFailure)
                    return Result.Failure<AddReviewResponse>(
                        proofOwner.ErrorKey,
                        proofOwner.ErrorArguments);

                var proofBiz = ReviewBusinessRules.ProofMustMatchBusiness(proof, businessId);
                if (proofBiz.IsFailure)
                    return Result.Failure<AddReviewResponse>(
                        proofBiz.ErrorKey,
                        proofBiz.ErrorArguments);
            }
            if (!business.IsActive)
                return Result.Failure<AddReviewResponse>("This business is no longer active.");
             */

            var proof = request.ProofId.HasValue
                ? await _proofRepository.GetByIdAsync(request.ProofId.Value, ct)
                : null;

            if (request.ProofId.HasValue)
            {
                if (proof is null)
                    return Result<ReviewResponseDto>.Failure(
                        AppErrorsCataloge.NotFound(
                            ProofMessage.NotFound,
                            _localizer[ProofMessage.NotFound]));

                if (proof.Status == ProofStatus.Rejected)
                    return Result<ReviewResponseDto>.Failure(
                        AppErrorsCataloge.Validation(
                            ProofMessage.RejectFailed,
                            _localizer[ProofMessage.RejectFailed]));
            }

            var review = Review.Create(
                userId,
                request.BranchId,
                request.FoodRating,
                request.ServiceRating,
                request.CleanlinessRating,
                request.AmbienceRating,
                request.ValueRating,
                $"Review for {branch.Business.Name} Branch",
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
    Guid adminId,
    Guid reviewId,
    AdminReviewActionRequest request,
    CancellationToken ct = default)
    {
        try
        {
            var review = await _readRepo.GetByIdAsync(reviewId, ct);

            if (review is null)
                return Result.Failure(
                    AppErrorsCataloge.NotFound(
                        ReviewMessage.NotFound,
                        _localizer[ReviewMessage.NotFound]));

            if (review.Status != ReviewStatus.Pending)
                return Result.Failure(
                    AppErrorsCataloge.Conflict(
                        ReviewMessage.AlreadyProcessed,
                        _localizer[ReviewMessage.AlreadyProcessed]));

            review.Approve(adminId, request.Note);

            await _writeRepo.UpdateAsync(review, ct);

            var branch = await _branchRepo.GetByIdAsync(review.BranchId, ct);

            if (branch is not null)
            {
                branch.DecrementPendingReviews();

                await _uow.SaveChangesAsync(ct);

                var approvedReviews =
                    await _readRepo.GetApprovedRatingsAsync(
                        branch.Id,
                        ct);

                var pendingCount =
                    await _readRepo.CountAsync(
                        r => r.BranchId == branch.Id &&
                             r.Status == ReviewStatus.Pending,
                        ct);

                branch.RecalculateRatings(
                    approvedReviews,
                    pendingCount);

                await _writeBranchRepo.UpdateAsync(branch, ct);

                await _uow.SaveChangesAsync(ct);
            }

            await _cache.RemoveAsync($"review:{reviewId}", ct);

            await _notifications.NotifyUserAsync(
                review.UserId,
                NotificationType.ReviewApproved,
                "Your review was approved",
                $"Your review '{review.Title}' has been approved and is now live.",
                review.Id.ToString(),
                "Review",
                ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(
                AppErrorsCataloge.Failure(
                    ex.ErrorCode,
                    _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error approving review {ReviewId}",
                reviewId);

            return Result.Failure(
                AppErrorsCataloge.Failure(
                    ReviewMessage.ApproveFailed,
                    _localizer[ReviewMessage.ApproveFailed]));
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

            var reason = request.Note ?? "Not specified reason";
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

    private async Task<Proof> ValidateProofAsync(Guid? proofId, CancellationToken ct)
    {
        if (proofId is null)
            throw new DomainException("Proof is required", ProofMessage.Required);

        var proof = await _proofRepository.GetByIdAsync(proofId.Value, ct);

        if (proof is null)
            throw new DomainException("Proof not found", ProofMessage.NotFound);

        if (proof.Status == ProofStatus.Rejected)
            throw new DomainException("Proof rejected", ProofMessage.RejectFailed);

        //if(proof.)
        //    throw new DomainException("Invalid proof type", ProofMessage.RejectFailed);

        return proof;
    }
}
