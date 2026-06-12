using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Dto;
namespace Review_Guard.Application.Feature.ReviewModul.Query.GetPendingReviews;
public sealed record GetPendingReviewsQuery(int PageNumber, int PageSize) : IRequest<Result<PagedResult<ReviewListItemDto>>>;
