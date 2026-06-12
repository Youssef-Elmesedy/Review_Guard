using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.NotificationModule.DTOs;

public sealed record NotificationDto(
    Guid                 Id,
    NotificationType     Type,
    NotificationTarget   Target,
    string               Title,
    string               Message,
    bool                 IsRead,
    DateTime?            ReadAt,
    string?              ReferenceId,
    string?              ReferenceType,
    DateTime             CreatedAt
);

public sealed record UnreadCountDto(int Count);
