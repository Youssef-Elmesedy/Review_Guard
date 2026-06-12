using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Services;
namespace Review_Guard.Application.Feature.ProofModul.Command.VerifyProof;
internal sealed class VerifyProofCommandHandler : IRequestHandler<VerifyProofCommand, Result>
{
    private readonly IWriteProofService _service;
    public VerifyProofCommandHandler(IWriteProofService service) => _service = service;
    public async Task<Result> Handle(VerifyProofCommand request, CancellationToken ct)
        => await _service.VerifyAsync(request.AdminId, request.ProofId, request.Request, ct);
}
