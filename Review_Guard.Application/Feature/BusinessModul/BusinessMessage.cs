namespace Review_Guard.Application.Feature.BusinessModul;

public static class BusinessMessage
{
    public const string BusinessNotActive = "Business.NotActive";
    public const string BusinessNotFound = "Business.NotFound";

    public const string BusinessAlreadyExists = "Business.AlreadyExists";

    public const string InvalidBusinessData = "Business.InvalidData";

    public const string BusinessOperationFailed = "Business.OperationFailed";

    public const string CreateBusinessFailed = "Business.CreateBusinessFailed";

    public const string BusinessFetchError = "Business.FetchError";

    public const string NameExistsForOwnerAsync = "Business.NameExistsForOwnerAsync";

    public const string BusinessCategoryNotFound = "Business.BusinessCategoryNotFound";

    public const string BusinessSearchError = "Business.Erroroccurredwhilesearchingbusinessesbyname";

    public const string BranchReviewFetchError = "Business.Erroroccurredwhilefetchingbranchreviews";

    public const string BusinessWithReviewsFetchError = "Business.ErroroccurredwhilefetchingbusinessesWithReviews";

    public const string BusinessWithBranchsFetchError = "Business.ErroroccurredwhilefetchingbusinessesWithBranchs";

    public const string BusinessWithReviewsAndBranchsFetchError = "Business.ErroroccurredwhilefetchingbusinessesWithReviewsAndBranchs";

    // branches Messages
    public const string BrancheNotFound = "Branch.NotFound";

    public const string BusinessUpdateFailed = "Business.UpdateFailed";

    public const string BusinessDeleteFailed = "Business.DeleteFailed";

    public const string BusinessApproveFailed = "Business.ApproveFailed";

    public const string BusinessRejectFailed = "Business.RejectFailed";

    public const string BusinessAlreadyProcessed = "Business.AlreadyProcessed";

    public const string CreatedBusinessExists = "Business.CreatedBusinessExists";
}
