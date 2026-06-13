using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Application.Feature.BusinessModul.Services;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command.UpdateBusiness;

internal sealed class UpdateBusinessCommandHandler : IRequestHandler<UpdateBusinessCommand, Result<UpdateBusinessResponse>>
{
    private readonly IWriteBusinessService _service;
    public UpdateBusinessCommandHandler(IWriteBusinessService service) => _service = service;
    public async Task<Result<UpdateBusinessResponse>> Handle(UpdateBusinessCommand request, CancellationToken ct)
        => await _service.UpdateBusinessAsync(request.Response, ct);
}
