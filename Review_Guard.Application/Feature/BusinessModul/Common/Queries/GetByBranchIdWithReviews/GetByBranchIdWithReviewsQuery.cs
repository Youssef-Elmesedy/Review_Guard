using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetByBranchIdWithReviews;

public record GetByBranchIdWithReviewsQuery(Guid branchId) : IRequest<Result<BranchWithReviewsDto>>;
