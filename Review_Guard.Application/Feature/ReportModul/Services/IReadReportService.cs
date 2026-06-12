using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Dto;

namespace Review_Guard.Application.Feature.ReportModul.Services;

public interface IReadReportService
{
    Task<Result<ReportResponseDto>> GetByIdAsync(Guid reportId, CancellationToken ct = default);
    Task<Result<PagedResult<ReportListItemDto>>> GetAllReportsAsync(PaginationParams paging, CancellationToken ct = default);
    Task<Result<PagedResult<ReportListItemDto>>> GetOpenReportsAsync(PaginationParams paging, CancellationToken ct = default);
}
