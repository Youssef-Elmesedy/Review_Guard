using Review_Guard.Application.Feature.ProofModul;
using Review_Guard.Application.Feature.ReviewModul;
using Review_Guard.Application.Feature.ReviewModul.Dto;
using Review_Guard.Application.Feature.ReviewModul.Mapping;
using Review_Guard.Application.Feature.ReviewModul.Services;
using Review_Guard.Application.Feature.ReviewModul.Specification;
using Review_Guard.Domain.ValueObject;

namespace Review_Guard.Infrastructure.Implementation.Servcices.ReviewService;

internal sealed class WriteReviewService : IWriteReviewService
{
    private readonly IReadReviewRepository _readRepo;
    private readonly IWriteReviewRepository _writeRepo;
    private readonly IReadBranchRepository _branchRepo;
    private readonly IWriteBranchRepository _writeBranchRepo;
    private readonly IReadBusinessRepository _businessRepo;
    private readonly IReadUserRepository _readUserRepo;
    private readonly IWriteUserRepository _writeUserRepo;
    private readonly IReadProofRepository _proofRepository;
    private readonly INotificationService _notifications;
    private readonly IUnitOfWork _unitofwork;
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
        IReadProofRepository proofRepository,
        IReadUserRepository readUserRepo,
        IWriteUserRepository writeUserRepo)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _branchRepo = branchRepo;
        _writeBranchRepo = writeBranchRepo;
        _businessRepo = businessRepo;
        _notifications = notifications;
        _unitofwork = uow;
        _cache = cache;
        _logger = logger;
        _localizer = localizer;
        _proofRepository = proofRepository;
        _readUserRepo = readUserRepo;
        _writeUserRepo = writeUserRepo;
    }

    // ─────────────────────────────────────────────────────────
    // SUBMIT
    // ─────────────────────────────────────────────────────────
    public async Task<Result<ReviewResponseDto>> SubmitAsync(
        Guid userId,
        SubmitReviewRequest request,
        CancellationToken ct = default)
    {
        try
        {
            if (userId == Guid.Empty)
                return Fail<ReviewResponseDto>(ReviewMessage.LoginFirst);

            var business = await _businessRepo.GetByIdAsync(request.businessId, ct);
            if (business is null)
                return Fail<ReviewResponseDto>(BusinessMessage.BusinessNotFound);

            if (!business.IsActive)
                return Fail<ReviewResponseDto>(BusinessMessage.BusinessNotActive);

            var user = await _readUserRepo.GetByIdAsync(userId, ct);
            if (user is null)
                return Fail<ReviewResponseDto>(UserMessage.UserNotFound);

            ReviewBusinessRules.UserMustBeEligibleToReview(user);

            var branch = await _branchRepo.GetByIdAsync(request.BranchId, ct);
            if (branch is null)
                return Fail<ReviewResponseDto>(DomainMessagies.BranchNotFound);

            ReviewBusinessRules.UserCannotReviewOwnBusiness(user, branch);

            var duplicate = await _readRepo.AnyAsync(
                r => r.UserId == userId && r.BranchId == request.BranchId, ct);

            if (duplicate)
                return Fail<ReviewResponseDto>(ReviewMessage.AlreadyReviewed);

            var proof = await _proofRepository.GetByIdAsync(request.ProofId, ct);
            if (proof is null)
                return Fail<ReviewResponseDto>(ProofMessage.NotFound);

            ReviewBusinessRules.ProofMustMatchBusiness(proof, request.BranchId);

            var status = ReviewBusinessRules.DetermineReviewStatus(user, proof);

            var review = Review.Create(
                userId,
                request.BranchId,
                request.FoodRating,
                request.ServiceRating,
                request.CleanlinessRating,
                request.AmbienceRating,
                request.ValueRating,
                $"Review for {business.Name} Branch",
                request.Content,
                request.ProofId);

            review.MarkAsPending(status);

            await _unitofwork.ExecuteAsync(async () =>
            {
                await _writeRepo.AddAsync(review, ct);

                if (status == ReviewStatus.Pending)
                    branch.IncrementPendingReviews();

                await _writeBranchRepo.UpdateAsync(branch, ct);

                if (status == ReviewStatus.Approved)
                {
                    var approvedReviews = await _readRepo.GetApprovedRatingsAsync(branch.Id, ct);
                    var pendingCount = await _readRepo.CountAsync(
                        r => r.BranchId == branch.Id && r.Status == ReviewStatus.Pending, ct);

                    ApplyApprovalEffects(branch, user, approvedReviews, pendingCount);
                    await _writeUserRepo.UpdateAsync(user, ct);
                }

            }, ct);

            await InvalidateCache(review, branch, user, ct);

            await SendNotifications(review, branch, business);

            var dto = await _readRepo.ProjectFirstOrDefaultAsync(
                new ReviewByIdSpecification(review.Id),
                ReviewProjections.Full,
                ct);

            return Result<ReviewResponseDto>.Success(dto!);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error submitting review for user {UserId}", userId);
            return Fail<ReviewResponseDto>(ex.MessageKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting review for user {UserId}", userId);
            return Fail<ReviewResponseDto>(ReviewMessage.CreateFailed);
        }
    }

    // ─────────────────────────────────────────────────────────
    // APPROVE
    // ─────────────────────────────────────────────────────────
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
                return Result.Failure(AppErrorsCataloge
                    .NotFound(_localizer[ReviewMessage.NotFound]));

            if (review.Status != ReviewStatus.Pending)
                return Result.Failure(AppErrorsCataloge
                    .Validation(_localizer[ReviewMessage.AlreadyProcessed]));

            var branch = await _branchRepo.GetByIdAsync(review.BranchId, ct);
            var user = await _readUserRepo.GetByIdAsync(review.UserId, ct);

            if (branch is null)
                return Result.Failure(AppErrorsCataloge
                    .NotFound(_localizer[DomainMessagies.BranchNotFound]));

            if (user is null)
                return Result.Failure(AppErrorsCataloge
                    .NotFound(_localizer[UserMessage.UserNotFound]));

            await _unitofwork.ExecuteAsync(async () =>
            {
                review.Approve(adminId, request.Note);

                await _writeRepo.UpdateAsync(review, ct);

                branch.DecrementPendingReviews();

                var approvedReviews = await _readRepo.GetApprovedRatingsAsync(branch.Id, ct);

                var pendingCount = await _readRepo.CountAsync(
                    r => r.BranchId == branch.Id &&
                         r.Status == ReviewStatus.Pending,
                    ct);

                ApplyApprovalEffects(
                    branch,
                    user,
                    approvedReviews,
                    pendingCount);

                await _writeBranchRepo.UpdateAsync(branch, ct);
                await _writeUserRepo.UpdateAsync(user, ct);

            }, ct);

            await InvalidateCache(review, branch, user, ct);

            await _notifications.NotifyUserAsync(
                review.UserId,
                NotificationType.ReviewApproved,
                "Your review was approved",
                $"Your review '{review.Title}' is now live.",
                review.Id.ToString(),
                "Review",
                ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Approve review falied for Domain ex");
            return Result.Failure(AppErrorsCataloge
                .Validation(_localizer[ex.Message]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Approve review failed {ReviewId}", reviewId);
            return Result.Failure(AppErrorsCataloge
                .Failure(ex.Message));
        }
    }

    // ─────────────────────────────────────────────────────────
    // REJECT
    // ─────────────────────────────────────────────────────────
    public async Task<Result> RejectAsync(
        Guid adminId,
        Guid reviewId,
        AdminReviewActionRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var review = await _readRepo.GetByIdAsync(reviewId, ct);
            if (review is null)
                return Fail(ReviewMessage.NotFound);

            if (review.Status != ReviewStatus.Pending)
                return Fail(ReviewMessage.AlreadyProcessed);

            review.Reject(adminId, request.Note ?? "No reason");

            var branch = await _branchRepo.GetByIdAsync(review.BranchId, ct);
            var user = await _readUserRepo.GetByIdAsync(review.UserId, ct);

            branch?.DecrementPendingReviews();
            user?.DecreaseTrustScore(TrustScore.RejectionPenalty);

            await _unitofwork.ExecuteAsync(async () =>
            {
                await _writeRepo.UpdateAsync(review, ct);
                if (branch is not null) await _writeBranchRepo.UpdateAsync(branch, ct);
                if (user is not null) await _writeUserRepo.UpdateAsync(user, ct);
            }, ct);

            await InvalidateCache(review, branch, user, ct);

            await _notifications.NotifyUserAsync(
                review.UserId,
                NotificationType.ReviewRejected,
                "Your review was rejected",
                $"Reason: {request.Note}",
                review.Id.ToString(),
                "Review",
                ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reject review failed {ReviewId}", reviewId);
            return Fail(ReviewMessage.RejectFailed);
        }
    }
    // ── Delete ────────────────────────────────────────────────────────────
    public async Task<Result> DeleteAsync(
     Guid callerId,
     bool isAdmin,
     Guid reviewId,
     CancellationToken ct = default)
    {
        try
        {
            var review = await _readRepo.GetByIdAsync(reviewId, ct);

            if (review is null)
                return Result.Failure(
                    AppErrorsCataloge.NotFound(
                        _localizer[ReviewMessage.NotFound]));

            if (!isAdmin && review.UserId != callerId)
                return Result.Failure(
                    AppErrorsCataloge.Forbidden(
                        _localizer[ReviewMessage.Forbidden]));

            await _unitofwork.ExecuteAsync(async () =>
            {
                await _writeRepo.DeleteAsync(review, ct);
            }, ct);

            await _cache.RemoveAsync($"review:{reviewId}", ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error deleting review {ReviewId}",
                reviewId);

            return Result.Failure(
                AppErrorsCataloge.Failure(
                    _localizer[ReviewMessage.DeleteFailed]));
        }
    }

    // ─────────────────────────────────────────────────────────
    // DOMAIN EFFECTS (PURE)
    // ─────────────────────────────────────────────────────────
    private void ApplyApprovalEffects(
        Branch branch,
        User user,
        IEnumerable<(decimal Rating, decimal TrustWeight)> approvedReviews,
        int pendingCount)
    {
        branch.RecalculateRatings(approvedReviews, pendingCount); // Update SimpleAverageRating and WeightedAverageRating for Branch
        user.IncreaseTrustScore(TrustScore.ApprovalBonus); // Update TrustScore Value for User
        user.RecordReviewSubmission(); // Update Data Time Last Review and Incrument +1 Review Number for User
    }

    // ─────────────────────────────────────────────────────────
    // CACHE
    // ─────────────────────────────────────────────────────────
    private async Task InvalidateCache(Review review, Branch? branch, User? user, CancellationToken ct)
    {
        await _cache.RemoveByPrefixAsync($"review:", ct);

        if (branch is not null)
            await _cache.RemoveByPrefixAsync($"business:", ct);

        if (user is not null)
            await _cache.RemoveByPrefixAsync($"user:", ct);
    }

    // ─────────────────────────────────────────────────────────
    // NOTIFICATIONS
    // ─────────────────────────────────────────────────────────
    private async Task SendNotifications(Review review, Branch branch, Business business)
    {
        if (review.Status == ReviewStatus.Pending)
            await _notifications.NotifyAllAdminsAsync(
            NotificationType.NewReviewPending,
            "New review pending",
            $"Review at {branch.Address}",
            review.Id.ToString(), "Review");

        await _notifications.NotifyBusinessOwnerAsync(
            business.OwnerId,
            NotificationType.NewReviewOnBranch,
            "New review",
            $"Review at {branch.Address}",
            review.Id.ToString(), "Review");

        await _notifications.NotifyUserAsync(
            branch.ManagerId,
            NotificationType.NewReviewOnBranch,
            "New review",
            $"Review at {branch.Address}",
            review.Id.ToString(), "Review");
    }

    // ─────────────────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────────────────
    private Result<T> Fail<T>(string msg)
        => Result<T>.Failure(AppErrorsCataloge.Failure(_localizer[msg]));

    private Result Fail(string msg)
        => Result.Failure(AppErrorsCataloge.Forbidden(_localizer[msg]));
}