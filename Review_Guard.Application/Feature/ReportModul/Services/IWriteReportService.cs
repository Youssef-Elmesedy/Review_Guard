using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Dto;

namespace Review_Guard.Application.Feature.ReportModul.Services;

public interface IWriteReportService
{
    Task<Result<ReportResponseDto>> CreateAsync(Guid userId, CreateReportRequest request, CancellationToken ct = default);
    Task<Result> ResolveAsync(Guid adminId, Guid reportId, AdminReportActionRequest request, CancellationToken ct = default);
    Task<Result> DismissAsync(Guid adminId, Guid reportId, AdminReportActionRequest request, CancellationToken ct = default);
}
