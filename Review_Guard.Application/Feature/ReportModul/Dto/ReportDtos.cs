using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.ReportModul.Dto;

public sealed record CreateReportRequest(Guid ReviewId, ReportReason Reason, string Description);
public sealed record AdminReportActionRequest(string Note);

public sealed record ReportResponseDto(
    Guid         Id,
    Guid         ReportedByUserId,
    string       ReportedByUserFullName,
    Guid         ReviewId,
    ReportReason Reason,
    string       Description,
    ReportStatus Status,
    string?      AdminNote,
    DateTime     CreatedAt
);

public sealed record ReportListItemDto(
    Guid         Id,
    string       ReportedByUserFullName,
    ReportReason Reason,
    ReportStatus Status,
    DateTime     CreatedAt
);
