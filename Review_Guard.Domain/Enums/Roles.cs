namespace Review_Guard.Domain.Enums;

// user Roles With Create
public enum Roles
{
    SuperAdmin = 1, // Super admins have full access to all system features, including user and business management, review moderation, and analytics.
    Admin = 2,
    BusinessOwner = 3, // Business owners can manage their business profiles, respond to reviews, and access business-specific analytics.
    BranchManager = 4,
    User = 5, // Regular users can submit reviews, view business profiles, and interact with other users' reviews.
}

// Account With Create Chengies With Admin
public enum AccountStatus
{
    PendingVerification = 1, // Account is pending email verification.
    Active = 2, // Account is active and in good standing.
    Suspended = 3,// Account is temporarily suspended, restricting access until resolved.
    Banned = 4, // Account is permanently banned, access is revoked.
}

/// <summary>
/// Derived classification from TrustScore.
/// Drives review submission rules (proof required, auto-approve eligibility).
/// </summary>
public enum UserLevel
{
    /// <summary>TrustScore 0–39: proof required + admin approval always needed.</summary>
    LowTrust = 1,

    /// <summary>TrustScore 40–69: proof required, approval based on RiskScore.</summary>
    Normal = 2,

    /// <summary>TrustScore 70–100: auto-approved when RiskScore is low.</summary>
    Trusted = 3
}

// RiskLevel is derived from RiskScore and drives review moderation rules (manual review, auto-flagging).
public enum RiskLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

// ActivityType classifies user actions for logging and risk assessment purposes.
public enum ActivityType
{
    Login = 1,
    Register = 2,
    SubmitReview = 3,
    UploadProof = 4,
    ReportReview = 5,
    EmailVerified = 6,
    PasswordChanged = 7,
    ProfileUpdated = 8,
    SuspiciousAction = 9,
    Logout = 10
}

// ReviewStatus represents the lifecycle stages of a review.
public enum ReviewStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Flagged = 4,
    UnderReview = 5
}

// BusinessStatus represents the approval and activity status of a business page.
public enum BusinessStatus
{
    /// <summary>Awaiting admin review of commercial registration and tax documents.</summary>
    PendingApproval = 1,
    /// <summary>Admin has approved the business page — it is fully active.</summary>
    Active = 2,
    /// <summary>Admin rejected the submission (e.g. invalid documents).</summary>
    Rejected = 3,
    /// <summary>Business page has been deactivated by owner or admin.</summary>
    Inactive = 4
}

// CategoryStatus represents the approval status of a business category.
public enum CategoryStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}

// ReportStatus represents the lifecycle of a user report against a review or business.
public enum ReportStatus
{
    Open = 1,
    UnderReview = 2,
    Resolved = 3,
    Dismissed = 4
}

// ReportReason classifies the reasons why a user might report a review or business, guiding admin actions.
public enum ReportReason
{
    FakeReview = 1,
    Spam = 2,
    Offensive = 3,
    Misleading = 4,
    IrrelevantContent = 5,
    Other = 6
}

// ProofType
public enum ProofType
{
    Invoice = 1,
    Order = 2,
    Receipt = 3,
    BookingConfirmation = 4
}

// Proof Status
public enum ProofStatus
{
    Pending = 1,
    Verified = 2,
    Rejected = 3
}
public enum RewardType
{
    Bronze = 1,
    Silver = 2,
    Gold = 3
}

public enum TokenType
{
    VerificationEmail = 1,
    ResetPassword = 2,
}

public enum VerificationCodeType
{
    EmailVerification = 1,
    PasswordReset = 2,
    TwoFactor = 3
}
public enum MediaType
{
    Business = 1,
    Branch = 2
}

public enum ErrorType
{
    Validation,
    NotFound,
    Unauthorized,
    Forbidden,
    Conflict,
    Failure
}

public enum MediaOwnerType
{
    Business = 1,
    Branch = 2,
    User = 3,
    Proof = 4
}