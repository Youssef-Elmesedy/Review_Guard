using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Services;
namespace Review_Guard.Application.Feature.ReportModul.Command.ResolveReport;
internal sealed class ResolveReportCommandHandler : IRequestHandler<ResolveReportCommand, Result>
{
    private readonly IWriteReportService _service;
    public ResolveReportCommandHandler(IWriteReportService service) => _service = service;
    public async Task<Result> Handle(ResolveReportCommand request, CancellationToken ct)
        => await _service.ResolveAsync(request.AdminId, request.ReportId, request.Request, ct);
}
