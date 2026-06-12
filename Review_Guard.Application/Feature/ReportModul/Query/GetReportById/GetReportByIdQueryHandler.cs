using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Dto;
using Review_Guard.Application.Feature.ReportModul.Services;
namespace Review_Guard.Application.Feature.ReportModul.Query.GetReportById;
internal sealed class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, Result<ReportResponseDto>>
{
    private readonly IReadReportService _service;
    public GetReportByIdQueryHandler(IReadReportService service) => _service = service;
    public async Task<Result<ReportResponseDto>> Handle(GetReportByIdQuery request, CancellationToken ct)
        => await _service.GetByIdAsync(request.ReportId, ct);
}
