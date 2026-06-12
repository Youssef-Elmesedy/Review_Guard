using Review_Guard.Application.Feature.ReportModul.Dto;
using Review_Guard.Domain.Entities;
using System.Linq.Expressions;

namespace Review_Guard.Application.Feature.ReportModul.Mapping;

public static class ReportProjections
{
    public static Expression<Func<Report, ReportResponseDto>> Full =>
        r => new ReportResponseDto(
            r.Id, r.ReportedByUserId, r.ReportedByUser.FullName,
            r.ReviewId, r.Reason, r.Description, r.Status, r.AdminNote, r.CreatedAt);

    public static Expression<Func<Report, ReportListItemDto>> ListItem =>
        r => new ReportListItemDto(r.Id, r.ReportedByUser.FullName, r.Reason, r.Status, r.CreatedAt);
}
