using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;
using Review_Guard.Application.Feature.ProofModul.Services;
namespace Review_Guard.Application.Feature.ProofModul.Command.SubmitProofByFile;
internal sealed class SubmitProofByFileCommandHandler : IRequestHandler<SubmitProofByFileCommand, Result<ProofResponseDto>>
{
    private readonly IWriteProofService _service;
    public SubmitProofByFileCommandHandler(IWriteProofService service) => _service = service;
    public async Task<Result<ProofResponseDto>> Handle(SubmitProofByFileCommand request, CancellationToken ct)
        => await _service.SubmitByFileAsync(request.UserId, request.Request, ct);
}
