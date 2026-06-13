using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Services;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command.RejectBusiness;

internal sealed class RejectBusinessCommandHandler : IRequestHandler<RejectBusinessCommand, Result>
{
    private readonly IWriteBusinessService _service;
    public RejectBusinessCommandHandler(IWriteBusinessService service) => _service = service;
    public async Task<Result> Handle(RejectBusinessCommand request, CancellationToken ct)
        => await _service.RejectBusinessAsync(request.BusinessId, request.Reason, ct);
}
