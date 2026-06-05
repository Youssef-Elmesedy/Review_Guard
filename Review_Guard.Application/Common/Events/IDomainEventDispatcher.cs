using Review_Guard.Domain.Common;

namespace Review_Guard.Application.Common.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events);
}
