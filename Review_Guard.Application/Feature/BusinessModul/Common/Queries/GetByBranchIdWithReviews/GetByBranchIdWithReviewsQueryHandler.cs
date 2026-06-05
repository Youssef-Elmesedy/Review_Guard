using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Application.Feature.BusinessModul.Services;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetByBranchIdWithReviews;

internal sealed class GetByBranchIdWithReviewsQueryHandler : IRequestHandler<GetByBranchIdWithReviewsQuery, Result<BranchWithReviewsDto>>
{
    private readonly IReadBusinessService _businessService;

    public GetByBranchIdWithReviewsQueryHandler(IReadBusinessService businessService)
    {
        _businessService = businessService;
    }

    public async Task<Result<BranchWithReviewsDto>> Handle(GetByBranchIdWithReviewsQuery request, CancellationToken ct)
    {
        var result = await _businessService.GetByBranchIdWithReview(request.branchId, ct);

        return result;
    }
}
