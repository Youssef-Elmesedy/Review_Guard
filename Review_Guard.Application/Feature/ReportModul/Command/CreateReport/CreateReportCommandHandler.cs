using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Dto;
using Review_Guard.Application.Feature.ReportModul.Services;
namespace Review_Guard.Application.Feature.ReportModul.Command.CreateReport;
internal sealed class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Result<ReportResponseDto>>
{
    private readonly IWriteReportService _service;
    public CreateReportCommandHandler(IWriteReportService service) => _service = service;
    public async Task<Result<ReportResponseDto>> Handle(CreateReportCommand request, CancellationToken ct)
        => await _service.CreateAsync(request.UserId, request.Request, ct);
}
