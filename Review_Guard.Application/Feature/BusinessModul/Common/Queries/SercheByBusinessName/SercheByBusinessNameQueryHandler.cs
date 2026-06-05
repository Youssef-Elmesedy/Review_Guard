using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Application.Feature.BusinessModul.Services;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.SercheByBusinessName;

internal sealed class SercheByBusinessNameQueryHandler : IRequestHandler<SercheByBusinessNameQuery, Result<PagedResult<BusinessListWithBranchsDto>>>
{
    private readonly IReadBusinessService _readBusinessService;

    public SercheByBusinessNameQueryHandler(IReadBusinessService readBusinessService)
    {
        _readBusinessService = readBusinessService;
    }

    public async Task<Result<PagedResult<BusinessListWithBranchsDto>>> Handle(SercheByBusinessNameQuery request, CancellationToken ct = default)
    {
        var paginationParams = new PaginationParams { PageNumber = request.pageNumber, PageSize = request.pageSize };

        var result = await _readBusinessService.SearchByBusinessName(request.businessName, paginationParams, ct);

        return result;
    }
}
