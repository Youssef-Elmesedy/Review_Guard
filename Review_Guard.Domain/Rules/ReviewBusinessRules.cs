using Review_Guard.Domain.Entities;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Rules;

public static class ReviewBusinessRules
{
    public static void UserCannotReviewOwnBusiness(User user, Branch branch)
    {
        if (branch.Business.IsOwnedBy(user.Id))
            throw new DomainException(
                "You cannot review your own business.",
                DomainMessagies.UserCannotReviewOwnBusiness);

        if (branch.IsManagedBy(user.Id))
            throw new DomainException(
                "You cannot review a business you manage.",
                DomainMessagies.UserCannotReviewManagedBusiness);
    }

    public static void UserMustBeEligibleToReview(User user, int maxPerDay = 5)
    {
        user.ValidateCanSubmitReview(maxPerDay);
    }

    public static void UserHasNotAlreadyReviewedBusiness(bool hasExisting)
    {
        if (hasExisting)
            throw new DomainException(
                "You have already submitted a review for this business.",
                DomainMessagies.AlreadyReviewed);
    }

    public static void ProofMustBelongToUser(Proof proof, Guid userId)
    {
        if (proof.UserId != userId)
            throw new DomainException(
                "The provided proof does not belong to you.",
                DomainMessagies.Unauthorized);
    }

    public static void ProofMustMatchBusiness(Proof proof, Guid branchid)
    {
        if (proof.BranchId != branchid)
            throw new DomainException(
                "The provided proof does not match this business.",
                DomainMessagies.ProofBusinessMismatch);
    }

    public static void ProofMustBeVerified(Proof proof)
    {
        //if (!proof.VerifiedByAdminId.HasValue)
        //    throw new DomainException(
        //        "Your proof of purchase must be verified before submitting a review.",
        //        DomainMessagies.ProofNotVerified);
    }

    public static void ProofRequiredForUserLevel(User user, Proof? proof)
    {
        if (!user.TrustScore.RequiresProof)
            return;

        if (proof is null)
            throw new DomainException(
                $"Users at {user.Level} level must supply proof of purchase.",
                DomainMessagies.ProofRequired);

        ProofMustBeVerified(proof);
    }

    public static ReviewStatus DetermineReviewStatus(User user, Proof? proof)
    {
        if (user.Level == UserLevel.Trusted &&
            proof is not null &&
            proof.Status == ProofStatus.Verified)
        {
            return ReviewStatus.Approved; // auto approve
        }

        return ReviewStatus.Pending; // admin review
    }

    public static void ValidateRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException(
                "Rating must be between 1 and 5.",
                DomainMessagies.InvalidRatingValue);
    }
}