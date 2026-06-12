using Review_Guard.Application.Abstractions.Repositories.GenericRepository;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Abstractions.Repositories.NotificationRepository;

public interface IWriteNotificationRepository : IGenericWriteRepository<Notification>
{
    Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct = default);
    Task MarkAllAsReadAsync(Guid recipientId, bool isAdmin, CancellationToken ct = default);
}
