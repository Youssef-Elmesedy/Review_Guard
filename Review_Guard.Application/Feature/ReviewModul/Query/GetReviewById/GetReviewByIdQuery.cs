using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Dto;
namespace Review_Guard.Application.Feature.ReviewModul.Query.GetReviewById;
public sealed record GetReviewByIdQuery(Guid ReviewId) : IRequest<Result<ReviewResponseDto>>;
