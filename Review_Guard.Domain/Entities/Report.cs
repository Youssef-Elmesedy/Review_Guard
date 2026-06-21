using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Entities;

public class Report : BaseEntity
{
    private Report() { }

    public Guid ReportedByUserId { get; private set; }
    public Guid ReviewId { get; private set; }
    public ReportReason Reason { get; private set; }
    public string Description { get; private set; } = default!;
    public ReportStatus Status { get; private set; } = ReportStatus.Open;

    public string? AdminNote { get; private set; }
    public Guid? ResolvedByAdminId { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    // Navigation
    public User ReportedByUser { get; private set; } = default!;
    public Review Review { get; private set; } = default!;

    public static Report Create(Guid reportedByUserId, Guid reviewId, ReportReason reason, string description)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException(
            DomainMessagies.DescriptionRequired);
        return new Report
        {
            ReportedByUserId = reportedByUserId,
            ReviewId = reviewId,
            Reason = reason,
            Description = description.Trim()
        };
    }

    public void Resolve(Guid adminId, string note)
    {
        Status = ReportStatus.Resolved;
        AdminNote = note;
        ResolvedByAdminId = adminId;
        ResolvedAt = DateTime.UtcNow;

        SetUpdatedAt();
    }

    public void Dismiss(Guid adminId, string note)
    {
        Status = ReportStatus.Dismissed;
        AdminNote = note;
        ResolvedByAdminId = adminId;
        ResolvedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void MarkUnderReview()
    {
        Status = ReportStatus.UnderReview;

        SetUpdatedAt();
    }
}
