namespace Review_Guard.Infrastructure.Implementation.Repositories.NotificationRepository;

internal sealed class WriteNotificationRepository
    : GenericWriteRepository<Notification>, IWriteNotificationRepository
{
    public WriteNotificationRepository(AppDbContext ctx) : base(ctx) { }

    public async Task AddRangeAsync(
        IEnumerable<Notification> notifications, CancellationToken ct = default)
    {
        await _context.Notifications.AddRangeAsync(notifications, ct);
    }

    public async Task MarkAllAsReadAsync(
        Guid recipientId, bool isAdmin, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        if (isAdmin)
        {
            await _context.Notifications
                .Where(n => n.AdminId == recipientId && !n.IsRead)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(n => n.IsRead, true)
                    .SetProperty(n => n.ReadAt, now), ct);
        }
        else
        {
            await _context.Notifications
                .Where(n => n.UserId == recipientId && !n.IsRead)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(n => n.IsRead, true)
                    .SetProperty(n => n.ReadAt, now), ct);
        }
    }
}
