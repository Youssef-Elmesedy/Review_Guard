// FILE: Review_Guard.Application / Feature / AdminModule / DTOs / AdminDtos.cs

namespace Review_Guard.Application.Feature.AdminModule.DTOs;

// ── Profile ────────────────────────────────────────────────────────────────

public sealed record AdminProfileResponse(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    bool IsActive,
    int TotalActionsPerformed,
    DateTime? LastLoginAt,
    DateTime CreatedAt
);

// ── Dashboard ──────────────────────────────────────────────────────────────

public sealed record AdminDashboardResponse(
    // Users
    int TotalUsers,
    int ActiveUsers,
    int SuspendedUsers,
    int BannedUsers,

    // Reviews
    int TotalReviews,
    int PendingReviews,
    int ApprovedReviews,
    int RejectedReviews,

    // Businesses
    int TotalBusinesses,
    int PendingBusinesses,
    int ActiveBusinesses,

    // Reports
    int OpenReports,

    // Snapshot date
    DateTime GeneratedAt
);

// ── Update Admin Profile ───────────────────────────────────────────────────

public sealed record UpdateAdminProfileRequest(
    string? FullName
);

public sealed record ChangeAdminPasswordRequest(
    string OldPassword,
    string NewPassword,
    string ConfirmNewPassword
);

// ── Broadcast Notification ────────────────────────────────────────────────

public sealed record BroadcastNotificationRequest(
    string Title,
    string Message,
    string? ReferenceId = null,
    string? ReferenceType = null
);
