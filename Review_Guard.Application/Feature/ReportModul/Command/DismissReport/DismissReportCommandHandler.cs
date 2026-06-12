using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Services;
namespace Review_Guard.Application.Feature.ReportModul.Command.DismissReport;
internal sealed class DismissReportCommandHandler : IRequestHandler<DismissReportCommand, Result>
{
    private readonly IWriteReportService _service;
    public DismissReportCommandHandler(IWriteReportService service) => _service = service;
    public async Task<Result> Handle(DismissReportCommand request, CancellationToken ct)
        => await _service.DismissAsync(request.AdminId, request.ReportId, request.Request, ct);
}
