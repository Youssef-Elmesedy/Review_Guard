using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetAllByCategoryId;

public record GetAllBusinessWithBranchsByCategoryIdQuery(int page, int pageSize, Guid categoryId)
    : IRequest<Result<PagedResult<BusinessListWithBranchsDto>>>;
