// FILE: Review_Guard.Infrastructure / Implementation / Servcices /
//       AdminService / AdminService.cs

using Review_Guard.Application.Feature.AdminModule;
using Review_Guard.Application.Feature.AdminModule.DTOs;
using Review_Guard.Application.Feature.AdminModule.Services;

namespace Review_Guard.Infrastructure.Implementation.Servcices.AdminService;

internal sealed class AdminService : IAdminService
{
    private readonly IReadAdminRepository _readAdminRepo;
    private readonly IWriteAdminRepository _writeAdminRepo;
    private readonly INotificationService _notifications;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly AppDbContext _db;
    private readonly ILogger<AdminService> _logger;
    private readonly IStringLocalizer<AdminService> _localizer;

    public AdminService(
        IReadAdminRepository readAdminRepo,
        IWriteAdminRepository writeAdminRepo,
        INotificationService notifications,
        IUnitOfWork uow,
        AppDbContext db,
        ILogger<AdminService> logger,
        IStringLocalizer<AdminService> localizer,
        IPasswordHasher passwordHasher)
    {
        _readAdminRepo = readAdminRepo;
        _writeAdminRepo = writeAdminRepo;
        _notifications = notifications;
        _uow = uow;
        _db = db;
        _logger = logger;
        _localizer = localizer;
        _passwordHasher = passwordHasher;
    }

    // ── Get Profile ────────────────────────────────────────────────────────
    public async Task<Result<AdminProfileResponse>> GetProfileAsync(
        Guid adminId, CancellationToken ct = default)
    {
        try
        {
            var admin = await _readAdminRepo.GetByIdAsync(adminId, ct);
            if (admin is null)
                return Result<AdminProfileResponse>.Failure(
                    AppErrorsCataloge.NotFound(_localizer[AdminMessage.NotFound]));

            return Result<AdminProfileResponse>.Success(MapToProfile(admin));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetProfileAsync failed for admin {AdminId}", adminId);
            return Result<AdminProfileResponse>.Failure(
                AppErrorsCataloge.Failure(_localizer[AdminMessage.UpdateFailed]));
        }
    }

    // ── Dashboard ──────────────────────────────────────────────────────────
    public async Task<Result<AdminDashboardResponse>> GetDashboardAsync(
        Guid adminId, CancellationToken ct = default)
    {
        try
        {
            // Verify admin exists
            var exists = await _readAdminRepo.AnyAsync(a => a.Id == adminId, ct);
            if (!exists)
                return Result<AdminDashboardResponse>.Failure(
                    AppErrorsCataloge.NotFound(_localizer[AdminMessage.NotFound]));

            // Users
            var totalUsers = await _db.Users.CountAsync(ct);
            var activeUsers = await _db.Users.CountAsync(u => u.Status == AccountStatus.Active, ct);
            var suspendedUsers = await _db.Users.CountAsync(u => u.Status == AccountStatus.Suspended, ct);
            var bannedUsers = await _db.Users.CountAsync(u => u.Status == AccountStatus.Banned, ct);

            // Reviews
            var totalReviews = await _db.Reviews.CountAsync(ct);
            var pendingReviews = await _db.Reviews.CountAsync(r => r.Status == ReviewStatus.Pending, ct);
            var approvedReviews = await _db.Reviews.CountAsync(r => r.Status == ReviewStatus.Approved, ct);
            var rejectedReviews = await _db.Reviews.CountAsync(r => r.Status == ReviewStatus.Rejected, ct);

            // Businesses
            var totalBusinesses = await _db.Businesses.CountAsync(ct);
            var pendingBusinesses = await _db.Businesses.CountAsync(b => b.Status == BusinessStatus.PendingApproval, ct);
            var activeBusinesses = await _db.Businesses.CountAsync(b => b.Status == BusinessStatus.Active, ct);

            // Reports
            var openReports = await _db.Reports.CountAsync(r => r.Status == ReportStatus.Open, ct);

            var dashboard = new AdminDashboardResponse(
                TotalUsers: totalUsers,
                ActiveUsers: activeUsers,
                SuspendedUsers: suspendedUsers,
                BannedUsers: bannedUsers,

                TotalReviews: totalReviews,
                PendingReviews: pendingReviews,
                ApprovedReviews: approvedReviews,
                RejectedReviews: rejectedReviews,

                TotalBusinesses: totalBusinesses,
                PendingBusinesses: pendingBusinesses,
                ActiveBusinesses: activeBusinesses,

                OpenReports: openReports,

                GeneratedAt: DateTime.UtcNow
            );

            return Result<AdminDashboardResponse>.Success(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetDashboardAsync failed for admin {AdminId}", adminId);
            return Result<AdminDashboardResponse>.Failure(
                AppErrorsCataloge.Failure(_localizer[AdminMessage.DashboardFetchFailed]));
        }
    }

    // ── Update Profile ─────────────────────────────────────────────────────
    public async Task<Result> UpdateProfileAsync(
        Guid adminId, UpdateAdminProfileRequest request, CancellationToken ct = default)
    {
        try
        {
            var admin = await _readAdminRepo.GetByIdAsync(adminId, ct);
            if (admin is null)
                return Result.Failure(
                    AppErrorsCataloge.NotFound(_localizer[AdminMessage.NotFound]));

            admin.UpdateProfile(request.FullName);

            await _uow.ExecuteAsync(async () =>
            {
                await _writeAdminRepo.UpdateAsync(admin, ct);
            }, ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateProfileAsync failed for admin {AdminId}", adminId);
            return Result.Failure(
                AppErrorsCataloge.Failure(_localizer[AdminMessage.UpdateFailed]));
        }
    }

    // ── Change Password ───────────────────────────────────────────────────
    public async Task<Result> ChangePasswordAsync(
        Guid adminId, ChangeAdminPasswordRequest request, CancellationToken ct = default)
    {
        try
        {
            var admin = await _readAdminRepo.GetByIdAsync(adminId, ct);
            if (admin is null)
                return Result.Failure(
                    AppErrorsCataloge.NotFound(_localizer[AdminMessage.NotFound]));

            var isPasswordValid = _passwordHasher.VerifyPassword(request.OldPassword, admin.PasswordHash);

            if (!isPasswordValid)
                return Result.Failure(
                    AppErrorsCataloge.Failure(_localizer[AdminMessage.InvalidPassword]));

            if (request.NewPassword.Equals(request.OldPassword))
                return Result.Failure(
                    AppErrorsCataloge.Failure(_localizer[AdminMessage.NewPasswordSameAsOld]));

            if (request.NewPassword != request.ConfirmNewPassword)
                return Result.Failure(
                    AppErrorsCataloge.Failure(_localizer[AdminMessage.PasswordsDoNotMatch]));

            admin.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));

            await _uow.ExecuteAsync(async () =>
            {
                await _writeAdminRepo.UpdateAsync(admin, ct);
            }, ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "ChangePasswordAsync domain error for admin {AdminId}", adminId);
            return Result.Failure(
                AppErrorsCataloge.Failure(_localizer[ex.Message]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ChangePasswordAsync failed for admin {AdminId}", adminId);
            return Result.Failure(
                AppErrorsCataloge.Failure(_localizer[AdminMessage.ChangePasswordFailed]));
        }
    }

    // ── Broadcast Notification ─────────────────────────────────────────────
    public async Task<Result> BroadcastNotificationAsync(
        Guid adminId, BroadcastNotificationRequest request, CancellationToken ct = default)
    {
        try
        {
            // Verify caller is a valid admin
            var exists = await _readAdminRepo.AnyAsync(a => a.Id == adminId, ct);
            if (!exists)
                return Result.Failure(
                    AppErrorsCataloge.Unauthorized(_localizer[AdminMessage.Unauthorized]));

            await _notifications.NotifyAllUsersAsync(
                NotificationType.SystemAnnouncement,
                request.Title,
                request.Message,
                request.ReferenceId,
                request.ReferenceType,
                ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BroadcastNotificationAsync failed for admin {AdminId}", adminId);
            return Result.Failure(
                AppErrorsCataloge.Failure(_localizer[AdminMessage.BroadcastFailed]));
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────
    private static AdminProfileResponse MapToProfile(Admin admin) => new(
        Id: admin.Id,
        FullName: admin.FullName,
        Email: admin.Email,
        Role: admin.Role.ToString(),
        IsActive: admin.IsActive,
        TotalActionsPerformed: admin.TotalActionsPerformed,
        LastLoginAt: admin.LastLoginAt,
        CreatedAt: admin.CreatedAt
    );
}
