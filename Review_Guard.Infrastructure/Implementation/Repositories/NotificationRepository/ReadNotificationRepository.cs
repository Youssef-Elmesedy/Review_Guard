using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Infrastructure.Implementation.Repositories.NotificationRepository;

internal sealed class ReadNotificationRepository
    : GenericReadRepository<Notification>, IReadNotificationRepository
{
    public ReadNotificationRepository(AppDbContext ctx) : base(ctx) { }

    public async Task<IReadOnlyList<Notification>> GetForUserAsync(
        Guid userId, bool unreadOnly, PaginationParams paging, CancellationToken ct = default)
    {
        var query = _appDbContext.Notifications
            .Where(n => n.UserId == userId
                     && (n.Target == NotificationTarget.User
                      || n.Target == NotificationTarget.BusinessOwner));

        if (unreadOnly) query = query.Where(n => !n.IsRead);

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip(paging.Skip)
            .Take(paging.PageSize)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Notification>> GetForAdminAsync(
        Guid adminId, bool unreadOnly, PaginationParams paging, CancellationToken ct = default)
    {
        var query = _appDbContext.Notifications
            .Where(n => n.AdminId == adminId && n.Target == NotificationTarget.Admin);

        if (unreadOnly) query = query.Where(n => !n.IsRead);

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip(paging.Skip)
            .Take(paging.PageSize)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountUnreadForUserAsync(Guid userId, CancellationToken ct = default)
        => await _appDbContext.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead, ct);

    public async Task<int> CountUnreadForAdminAsync(Guid adminId, CancellationToken ct = default)
        => await _appDbContext.Notifications
            .CountAsync(n => n.AdminId == adminId && !n.IsRead, ct);
}
