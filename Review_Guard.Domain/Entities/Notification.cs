using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Domain.Entities;

public sealed class Notification : BaseEntity
{
    private Notification() { }

    // ── Recipient  ─────────────────────────────────
    public Guid?   UserId    { get; private set; }   // null when Admin-only
    public Guid?   AdminId   { get; private set; }   // null when User-only

    // ── Content ────────────────────────────────────
    public NotificationType Type    { get; private set; }
    public NotificationTarget Target { get; private set; }
    public string Title   { get; private set; } = default!;
    public string Message { get; private set; } = default!;

    // ── State ──────────────────────────────────────
    public bool      IsRead   { get; private set; }
    public DateTime? ReadAt   { get; private set; }

    // ── Optional reference ─────────────────────────
    public string? ReferenceId   { get; private set; }  // e.g. ReviewId, BusinessId
    public string? ReferenceType { get; private set; }  // "Review" | "Business" | …

    // ── Navigation ─────────────────────────────────
    public User?  User  { get; private set; }
    public Admin? Admin { get; private set; }

    // ── Factories ──────────────────────────────────

    public static Notification ForUser(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        string? referenceId   = null,
        string? referenceType = null)
        => new()
        {
            Id            = Guid.NewGuid(),
            UserId        = userId,
            Target        = NotificationTarget.User,
            Type          = type,
            Title         = title,
            Message       = message,
            IsRead        = false,
            ReferenceId   = referenceId,
            ReferenceType = referenceType,
            CreatedAt     = DateTime.UtcNow
        };

    public static Notification ForAdmin(
        Guid adminId,
        NotificationType type,
        string title,
        string message,
        string? referenceId   = null,
        string? referenceType = null)
        => new()
        {
            Id            = Guid.NewGuid(),
            AdminId       = adminId,
            Target        = NotificationTarget.Admin,
            Type          = type,
            Title         = title,
            Message       = message,
            IsRead        = false,
            ReferenceId   = referenceId,
            ReferenceType = referenceType,
            CreatedAt     = DateTime.UtcNow
        };

    public static Notification ForBusinessOwner(
        Guid ownerId,
        NotificationType type,
        string title,
        string message,
        string? referenceId   = null,
        string? referenceType = null)
        => new()
        {
            Id            = Guid.NewGuid(),
            UserId        = ownerId,
            Target        = NotificationTarget.BusinessOwner,
            Type          = type,
            Title         = title,
            Message       = message,
            IsRead        = false,
            ReferenceId   = referenceId,
            ReferenceType = referenceType,
            CreatedAt     = DateTime.UtcNow
        };

    // ── Behaviour ──────────────────────────────────
    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead  = true;
        ReadAt  = DateTime.UtcNow;
        SetUpdatedAt();
    }
}
