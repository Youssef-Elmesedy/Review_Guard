using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Application.Feature.BusinessModul.Services;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetAllBusiness;

internal sealed class GetAllBusinessQueryHandler
    : IRequestHandler<GetAllBusinessQuery, Result<PagedResult<BusinessListtDto>>>
{
    private readonly IReadBusinessService _businessService;

    public GetAllBusinessQueryHandler(IReadBusinessService businessService)
    {
        _businessService = businessService;
    }

    public async Task<Result<PagedResult<BusinessListtDto>>> Handle(GetAllBusinessQuery request, CancellationToken ct = default)
    {
        var paginationParams = new PaginationParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var result = await _businessService.GetAllBusiness(paginationParams, ct);

        return result;
    }
}