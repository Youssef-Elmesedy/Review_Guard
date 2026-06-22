namespace Review_Guard.Infrastructure.Implementation.Servcices.NotificationService;

internal sealed class NotificationService : INotificationService
{
    private readonly IWriteNotificationRepository _writeRepo;
    private readonly IReadAdminRepository _adminRepo;
    private readonly IReadUserRepository _userRepo;      // ← NEW
    private readonly IUnitOfWork _uow;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IWriteNotificationRepository writeRepo,
        IReadAdminRepository adminRepo,
        IReadUserRepository userRepo,                     // ← NEW
        IUnitOfWork uow,
        ILogger<NotificationService> logger)
    {
        _writeRepo = writeRepo;
        _adminRepo = adminRepo;
        _userRepo = userRepo;
        _uow = uow;
        _logger = logger;
    }

    // ── Single user ────────────────────────────────────────────────────────
    public async Task NotifyUserAsync(
        Guid userId, NotificationType type,
        string title, string message,
        string? referenceId = null, string? referenceType = null,
        CancellationToken ct = default)
    {
        try
        {
            var n = Notification.ForUser(userId, type, title, message, referenceId, referenceType);
            await _writeRepo.AddAsync(n, ct);
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to user {UserId}", userId);
        }
    }

    // ── Single admin ───────────────────────────────────────────────────────
    public async Task NotifyAdminAsync(
        Guid adminId, NotificationType type,
        string title, string message,
        string? referenceId = null, string? referenceType = null,
        CancellationToken ct = default)
    {
        try
        {
            var n = Notification.ForAdmin(adminId, type, title, message, referenceId, referenceType);
            await _writeRepo.AddAsync(n, ct);
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to admin {AdminId}", adminId);
        }
    }

    // ── Broadcast → all admins ─────────────────────────────────────────────
    public async Task NotifyAllAdminsAsync(
        NotificationType type, string title, string message,
        string? referenceId = null, string? referenceType = null,
        CancellationToken ct = default)
    {
        try
        {
            var admins = await _adminRepo.ListAllAsync(ct);

            var notifications = admins
                .Select(a => Notification.ForAdmin(a.Id, type, title, message, referenceId, referenceType))
                .ToList();

            await _writeRepo.AddRangeAsync(notifications, ct);
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast notification to all admins");
        }
    }

    // ── Business owner ─────────────────────────────────────────────────────
    public async Task NotifyBusinessOwnerAsync(
        Guid ownerId, NotificationType type,
        string title, string message,
        string? referenceId = null, string? referenceType = null,
        CancellationToken ct = default)
    {
        try
        {
            var n = Notification.ForBusinessOwner(ownerId, type, title, message, referenceId, referenceType);
            await _writeRepo.AddAsync(n, ct);
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to owner {OwnerId}", ownerId);
        }
    }

    // ── Broadcast → ALL active users ───────────────────────────────────────
    /// <summary>
    /// Sends a platform-wide notification to every Active user.
    /// Skips Banned / Suspended / PendingVerification accounts.
    /// Uses bulk AddRangeAsync for efficiency.
    /// </summary>
    public async Task NotifyAllUsersAsync(
        NotificationType type, string title, string message,
        string? referenceId = null, string? referenceType = null,
        CancellationToken ct = default)
    {
        try
        {
            // FindAsync(predicate) — inherited from GenericReadRepository<User>
            var activeUsers = await _userRepo.FindAsync(
                u => u.Status == AccountStatus.Active,
                ct);

            if (!activeUsers.Any())
            {
                _logger.LogWarning("NotifyAllUsersAsync: no active users found");
                return;
            }

            var notifications = activeUsers
                .Select(u => Notification.ForUser(
                    u.Id, type, title, message, referenceId, referenceType))
                .ToList();

            await _writeRepo.AddRangeAsync(notifications, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation(
                "NotifyAllUsersAsync: sent '{Title}' to {Count} users",
                title, notifications.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast notification to all users");
        }
    }
}