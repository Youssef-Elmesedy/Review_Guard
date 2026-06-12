using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Services;
namespace Review_Guard.Application.Feature.ProofModul.Command.RejectProof;
internal sealed class RejectProofCommandHandler : IRequestHandler<RejectProofCommand, Result>
{
    private readonly IWriteProofService _service;
    public RejectProofCommandHandler(IWriteProofService service) => _service = service;
    public async Task<Result> Handle(RejectProofCommand request, CancellationToken ct)
        => await _service.RejectAsync(request.AdminId, request.ProofId, request.Request, ct);
}
