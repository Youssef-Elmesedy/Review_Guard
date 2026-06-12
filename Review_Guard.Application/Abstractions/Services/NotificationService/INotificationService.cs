using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Abstractions.Services.NotificationService;

public interface INotificationService
{
    // ── Send to one user ───────────────────────────────────
    Task NotifyUserAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        string? referenceId   = null,
        string? referenceType = null,
        CancellationToken ct  = default);

    // ── Send to one admin ──────────────────────────────────
    Task NotifyAdminAsync(
        Guid adminId,
        NotificationType type,
        string title,
        string message,
        string? referenceId   = null,
        string? referenceType = null,
        CancellationToken ct  = default);

    // ── Broadcast to ALL admins ────────────────────────────
    Task NotifyAllAdminsAsync(
        NotificationType type,
        string title,
        string message,
        string? referenceId   = null,
        string? referenceType = null,
        CancellationToken ct  = default);

    // ── Send to a business owner ───────────────────────────
    Task NotifyBusinessOwnerAsync(
        Guid ownerId,
        NotificationType type,
        string title,
        string message,
        string? referenceId   = null,
        string? referenceType = null,
        CancellationToken ct  = default);
}
