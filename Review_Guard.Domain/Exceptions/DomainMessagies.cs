namespace Review_Guard.Domain.Exceptions;

public static class DomainMessagies
{
    // General validation messages
    public const string FullNameRequired =
    "Validation.FullNameRequired";

    public const string UserUniqueFullName =
        "Validation.UserUniqueFullName";

    public const string UserUniquePhone =
        "Validation.UserUniquePhone";

    public const string EmailRequired =
        "Validation.EmailRequired";

    public const string InvalidEmail =
        "Validation.InvalidEmail";

    public const string PasswordRequired =
        "Validation.PasswordRequired";

    public const string PasswordUnchanged =
        "Validation.PasswordUnchanged";

    public const string ConfirmPasswordRequired =
        "Validation.ConfirmPasswordRequired";

    public const string AddressRequired =
    "Validation.AddressRequired";


    public const string PhoneRequired =
        "Validation.PhoneRequired";

    public const string Unauthorized =
        "Validation.Unauthorized";

    public const string NotFound =
        "Validation.NotFound";

    // Business related messages
    public const string BusinessNameRequired =
        "Validation.BusinessNameRequired";

    public const string BusinessDescriptionRequired =
        "Validation.BusinessDescriptionRequired";

    public const string CommercialRegistrationNumberRequired =
        "Validation.CommercialRegistrationNumberRequired";

    public const string TaxNumberRequired =
        "Validation.TaxNumberRequired";

    public const string RejectionReasonRequired =
        "Validation.RejectionReasonRequired";

    public const string BranchNotFound =
        "Validation.BranchNotFound";

    public const string ProofFileUrlRequired =
        "Validation.ProofFileUrlRequired";

    public const string ProofOrderIdRequired =
        "Validation.ProofOrderIdRequired";

    public const string AlreadyVerified =
        "Validation.AlreadyVerified";

    public const string DescriptionRequired =
        "Validation.DescriptionRequired";

    public const string TitleIsRequired =
        "Validation.TitleIsRequired";

    public const string ContentIsRequired =
        "Validation.ContentIsRequired";

    public const string InvalidRatingValue =
        "Validation.InvalidRatingValue";

    public const string ContentIsRequiredLessThan20 =
        "Validation.ContentIsRequiredLessThan20";

    public const string EmailAlreadyVerified =
        "Validation.EmailAlreadyVerified";

    public const string AccountBanned =
        "Validation.AccountBanned";

    public const string SuspensionReasonRequired =
        "Validation.SuspensionReasonRequired";

    public const string BanReasonRequired =
    "Validation.BanReasonRequired";

    public const string AccountAlreadyBanned =
        "Validation.AccountAlreadyBanned";

    public const string AccountNotSuspended =
    "Validation.AccountNotSuspended";

    public const string AccountSuspended =
    "Validation.AccountSuspended";

    public const string EmailNotVerified =
        "Validation.EmailNotVerified";

    public const string ProofRequired =
    "Validation.ProofRequired";

    public const string DailyReviewLimitExceeded =
        "Validation.DailyReviewLimitExceeded";

    public const string ActivityDescriptionRequired =
    "Validation.ActivityDescriptionRequired";

    public const string SuspicionReasonRequired =
        "Validation.SuspicionReasonRequired";

    public const string MetadataKeyRequired =
        "Validation.MetadataKeyRequired";

    public const string AlreadyReviewed =
        "Validation.AlreadyReviewed";

    public const string ProofBusinessMismatch =
        "Validation.ProofBusinessMismatch";

    public const string ProofNotVerified =
        "Validation.ProofNotVerified";

    public const string EmailAlreadyExists =
       "Validation.EmailAlreadyExists";

    public const string AccountNotActive =
        "Validation.AccountNotActive";

    public const string TrustScoreTooLow =
        "Validation.TrustScoreTooLow";

    public const string TrustScoreInvalid =
        "Validation.TrustScoreInvalid";

    public const string ImageNotFound =
        "Business.ImageNotFound";

    public const string ImageUploadFailed =
        "Business.ImageUploadFailed";

    public const string MediaOwnership =
        "Validation.MediaOwnership";

    // Verification Token
    public const string InvalidVerificationCode =
        "Validation.InvalidVerificationCode";

    public const string VerificationCodeExpired =
        "Validation.VerificationCodeExpired";

    public const string VerificationCodeAlreadyUsed =
        "Validation.VerificationCodeAlreadyUsed";
}
