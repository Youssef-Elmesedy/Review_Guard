using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetAllBusiness;

public record GetAllBusinessQuery(int PageNumber, int PageSize) : IRequest<Result<PagedResult<BusinessListtDto>>>;
