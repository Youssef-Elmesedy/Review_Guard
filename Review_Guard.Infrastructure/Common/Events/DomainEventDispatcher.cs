using MediatR;
using Review_Guard.Domain.Common;

namespace Review_Guard.Infrastructure.Common.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task DispatchAsync(IEnumerable<IDomainEvent> events)
    {
        return Task.WhenAll(
            events.Select(e => _mediator.Publish(e))
        );
    }
}