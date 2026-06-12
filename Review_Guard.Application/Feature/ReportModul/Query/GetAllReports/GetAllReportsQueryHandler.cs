using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Dto;
using Review_Guard.Application.Feature.ReportModul.Services;
namespace Review_Guard.Application.Feature.ReportModul.Query.GetAllReports;
internal sealed class GetAllReportsQueryHandler : IRequestHandler<GetAllReportsQuery, Result<PagedResult<ReportListItemDto>>>
{
    private readonly IReadReportService _service;
    public GetAllReportsQueryHandler(IReadReportService service) => _service = service;
    public async Task<Result<PagedResult<ReportListItemDto>>> Handle(GetAllReportsQuery request, CancellationToken ct)
    {
        var paging = new PaginationParams { PageNumber = request.PageNumber, PageSize = request.PageSize };
        return request.OpenOnly
            ? await _service.GetOpenReportsAsync(paging, ct)
            : await _service.GetAllReportsAsync(paging, ct);
    }
}
