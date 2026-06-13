using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Services;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command.ApproveBusiness;

internal sealed class ApproveBusinessCommandHandler : IRequestHandler<ApproveBusinessCommand, Result>
{
    private readonly IWriteBusinessService _service;
    public ApproveBusinessCommandHandler(IWriteBusinessService service) => _service = service;
    public async Task<Result> Handle(ApproveBusinessCommand request, CancellationToken ct)
        => await _service.ApproveBusinessAsync(request.BusinessId, request.Note, ct);
}
