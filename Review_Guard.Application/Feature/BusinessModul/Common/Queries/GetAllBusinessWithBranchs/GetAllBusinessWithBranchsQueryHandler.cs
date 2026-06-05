using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Application.Feature.BusinessModul.Services;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetAll;

internal sealed class GetAllBusinessWithBranchsQueryHandler : IRequestHandler<GetAllBusinessWithBranchsQuery, Result<BusinessListWithBranchsDto>>
{
    private readonly IReadBusinessService _businessService;
    public GetAllBusinessWithBranchsQueryHandler(IReadBusinessService businessService)
    {
        _businessService = businessService;
    }
    public async Task<Result<BusinessListWithBranchsDto>> Handle(GetAllBusinessWithBranchsQuery request, CancellationToken cancellationToken)
    {
        var result = await _businessService.GetByBusinessIdWithBranchs(request.businessId, cancellationToken);

        return result;
    }
}
