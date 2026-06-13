using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Services;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command.DeleteBusiness;

internal sealed class DeleteBusinessCommandHandler : IRequestHandler<DeleteBusinessCommand, Result<bool>>
{
    private readonly IWriteBusinessService _service;
    public DeleteBusinessCommandHandler(IWriteBusinessService service) => _service = service;
    public async Task<Result<bool>> Handle(DeleteBusinessCommand request, CancellationToken ct)
        => await _service.DeleteBusinessAsync(request.BusinessId, ct);
}
