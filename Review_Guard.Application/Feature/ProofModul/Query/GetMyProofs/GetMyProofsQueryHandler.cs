using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul.Dto;
using Review_Guard.Application.Feature.ProofModul.Services;
namespace Review_Guard.Application.Feature.ProofModul.Query.GetMyProofs;
internal sealed class GetMyProofsQueryHandler : IRequestHandler<GetMyProofsQuery, Result<PagedResult<ProofListItemDto>>>
{
    private readonly IReadProofService _service;
    public GetMyProofsQueryHandler(IReadProofService service) => _service = service;
    public async Task<Result<PagedResult<ProofListItemDto>>> Handle(GetMyProofsQuery request, CancellationToken ct)
        => await _service.GetMyProofsAsync(request.UserId, new PaginationParams { PageNumber = request.PageNumber, PageSize = request.PageSize }, ct);
}
