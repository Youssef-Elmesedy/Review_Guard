using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;
using Review_Guard.Application.Feature.ProofModul.Services;
namespace Review_Guard.Application.Feature.ProofModul.Query.GetProofById;
internal sealed class GetProofByIdQueryHandler : IRequestHandler<GetProofByIdQuery, Result<ProofResponseDto>>
{
    private readonly IReadProofService _service;
    public GetProofByIdQueryHandler(IReadProofService service) => _service = service;
    public async Task<Result<ProofResponseDto>> Handle(GetProofByIdQuery request, CancellationToken ct)
        => await _service.GetByIdAsync(request.ProofId, ct);
}
