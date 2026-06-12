using Review_Guard.Application.Abstractions.Repositories.GenericRepository;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Entities;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Abstractions.Repositories.NotificationRepository;

public interface IReadNotificationRepository : IGenericReadRepository<Notification>
{
    Task<IReadOnlyList<Notification>> GetForUserAsync(
        Guid userId, bool unreadOnly, PaginationParams paging, CancellationToken ct = default);

    Task<IReadOnlyList<Notification>> GetForAdminAsync(
        Guid adminId, bool unreadOnly, PaginationParams paging, CancellationToken ct = default);

    Task<int> CountUnreadForUserAsync(Guid userId, CancellationToken ct = default);
    Task<int> CountUnreadForAdminAsync(Guid adminId, CancellationToken ct = default);
}
