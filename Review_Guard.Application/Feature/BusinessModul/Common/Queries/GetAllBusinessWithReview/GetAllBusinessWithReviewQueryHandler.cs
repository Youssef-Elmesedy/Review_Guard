using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Application.Feature.BusinessModul.Services;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetAllBusinessWithReview;

internal sealed class GetAllBusinessWithReviewQueryHandler : IRequestHandler<GetAllBusinessWithReviewQuery, Result<PagedResult<BusinessWithReviewDto>>>
{
    private readonly IReadBusinessService _businessService;

    public GetAllBusinessWithReviewQueryHandler(IReadBusinessService businessService)
    {
        _businessService = businessService;
    }

    public Task<Result<PagedResult<BusinessWithReviewDto>>> Handle(GetAllBusinessWithReviewQuery request, CancellationToken cancellationToken)
    {
        var paginationParams = new PaginationParams
        {
            PageNumber = request.pageNumber,
            PageSize = request.pageSize
        };

        var result = _businessService.GetAllBusinessWithReview(paginationParams, cancellationToken);

        return result;
    }
}
