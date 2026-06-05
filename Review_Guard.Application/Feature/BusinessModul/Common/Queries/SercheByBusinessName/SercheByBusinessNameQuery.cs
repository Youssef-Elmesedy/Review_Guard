using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.SercheByBusinessName;

public record SercheByBusinessNameQuery(int pageNumber, int pageSize, string businessName) : IRequest<Result<PagedResult<BusinessListWithBranchsDto>>>;
