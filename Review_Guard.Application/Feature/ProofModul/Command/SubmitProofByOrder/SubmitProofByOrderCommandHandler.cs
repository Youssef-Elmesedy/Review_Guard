using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;
using Review_Guard.Application.Feature.ProofModul.Services;
namespace Review_Guard.Application.Feature.ProofModul.Command.SubmitProofByOrder;
internal sealed class SubmitProofByOrderCommandHandler : IRequestHandler<SubmitProofByOrderCommand, Result<ProofResponseDto>>
{
    private readonly IWriteProofService _service;
    public SubmitProofByOrderCommandHandler(IWriteProofService service) => _service = service;
    public async Task<Result<ProofResponseDto>> Handle(SubmitProofByOrderCommand request, CancellationToken ct)
        => await _service.SubmitByOrderAsync(request.UserId, request.Request, ct);
}
