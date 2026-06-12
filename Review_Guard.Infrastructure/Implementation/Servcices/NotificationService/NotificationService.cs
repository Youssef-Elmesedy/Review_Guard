namespace Review_Guard.Infrastructure.Implementation.Servcices.NotificationService;

internal sealed class NotificationService : INotificationService
{
    private readonly IWriteNotificationRepository _writeRepo;
    private readonly IReadAdminRepository _adminRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IWriteNotificationRepository writeRepo,
        IReadAdminRepository adminRepo,
        IUnitOfWork uow,
        ILogger<NotificationService> logger)
    {
        _writeRepo = writeRepo;
        _adminRepo = adminRepo;
        _uow = uow;
        _logger = logger;
    }

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
}
