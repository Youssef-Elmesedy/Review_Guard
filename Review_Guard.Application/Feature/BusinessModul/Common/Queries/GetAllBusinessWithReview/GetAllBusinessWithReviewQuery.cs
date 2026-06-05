using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetAllBusinessWithReview;

public record GetAllBusinessWithReviewQuery(int pageNumber, int pageSize) : IRequest<Result<PagedResult<BusinessWithReviewDto>>>;
