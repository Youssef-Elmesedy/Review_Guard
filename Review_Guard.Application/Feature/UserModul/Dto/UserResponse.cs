using Microsoft.AspNetCore.Http;

namespace Review_Guard.Application.Feature.UserModul.Dto;

public sealed record UserProfileResponse(
    Guid Id,
    string FullName,
    string Email,
    bool IsEmailVerified,
    string? ProfileImageUrl,
    DateTime CreatedAt,

    int TotalReviews,
    decimal AverageRating,
    int ApprovedReviews,
    int RejectedReviews,

    decimal TrustScore,
    string Level
);

public sealed record UserReviewStats(
    int TotalReviews,
    decimal AverageRating,
    int ApprovedReviews,
    int RejectedReviews
);
// ── Admin views ──────────────────────────────────────────

public sealed record UserListItemDto(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    string Status,
    decimal TrustScore,
    string Level,
    int TotalReviews,
    DateTime CreatedAt,
    string? ProfileImageUrl
);

// ── Update profile request ───────────────────────────────

public sealed record UpdateProfileRequest(
    string? FullName,
    string? description,
    string? phone
);

// ── Change password request ──────────────────────────────

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
);

// ── Admin actions ─────────────────────────────────────────

public sealed record SuspendUserRequest(
    string Reason,
    DateTime? SuspendedUntil
);

public sealed record BanUserRequest(string Reason);

// ── User Activities ───────────────────────────────────────

public sealed record UserActivityDto(
    Guid Id,
    string ActivityType,
    string? Description,
    DateTime CreatedAt
);

// User Image Profile
public sealed class UpdateImageRequest
{
    public IFormFile FileImage { get; set; } = default!;
}