using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;
using Review_Guard.Application.Feature.ProofModul.Services;
namespace Review_Guard.Application.Feature.ProofModul.Query.GetPendingProofs;
internal sealed class GetPendingProofsQueryHandler : IRequestHandler<GetPendingProofsQuery, Result<PagedResult<ProofListItemDto>>>
{
    private readonly IReadProofService _service;
    public GetPendingProofsQueryHandler(IReadProofService service) => _service = service;
    public async Task<Result<PagedResult<ProofListItemDto>>> Handle(GetPendingProofsQuery request, CancellationToken ct)
        => await _service.GetPendingProofsAsync(new PaginationParams { PageNumber = request.PageNumber, PageSize = request.PageSize }, ct);
}
