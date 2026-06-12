using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Dto;
namespace Review_Guard.Application.Feature.ReviewModul.Query.GetMyReviews;
public sealed record GetMyReviewsQuery(Guid UserId, int PageNumber, int PageSize) : IRequest<Result<PagedResult<ReviewListItemDto>>>;
