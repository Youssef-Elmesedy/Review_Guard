using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Application.Feature.BusinessModul.Services;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetAllByCategoryId;

internal sealed class GetAllBusinessWithBranchsByCategoryIdQueryHandler : IRequestHandler<GetAllBusinessWithBranchsByCategoryIdQuery, Result<PagedResult<BusinessListWithBranchsDto>>>
{
    private readonly IReadBusinessService _businessService;

    public GetAllBusinessWithBranchsByCategoryIdQueryHandler(IReadBusinessService businessService)
    {
        _businessService = businessService;
    }

    public Task<Result<PagedResult<BusinessListWithBranchsDto>>> Handle(GetAllBusinessWithBranchsByCategoryIdQuery request, CancellationToken cancellationToken)
    {
        var paginationParams = new PaginationParams
        {
            PageNumber = request.page,
            PageSize = request.pageSize
        };

        var result = _businessService.GetAllBusinessWithBranchsByCategoryAsync(request.categoryId, paginationParams, cancellationToken);

        return result;
    }
}
