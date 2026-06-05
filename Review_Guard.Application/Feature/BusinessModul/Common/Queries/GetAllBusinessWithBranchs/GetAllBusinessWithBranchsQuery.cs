using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetAll;

public record GetAllBusinessWithBranchsQuery(Guid businessId) : IRequest<Result<BusinessListWithBranchsDto>>;
