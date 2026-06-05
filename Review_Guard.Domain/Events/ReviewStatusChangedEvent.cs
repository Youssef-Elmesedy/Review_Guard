using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Domain.Events;
/// <summary>
/// Raised when a review transitions from Pending → Approved/Rejected/Flagged.
/// </summary>
public sealed class ReviewStatusChangedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid ReviewId { get; init; }
    public Guid UserId { get; init; }
    public Guid BusinessId { get; init; }
    public ReviewStatus OldStatus { get; init; }
    public ReviewStatus NewStatus { get; init; }
    public Guid? AdminId { get; init; }
}
