using MediatR;
using Review_Guard.Application.Feature.ReviewModul.Dto;
namespace Review_Guard.Application.Feature.ReviewModul.Command.SubmitReview;

public sealed record SubmitReviewCommand(Guid UserId, SubmitReviewRequest Request) : IRequest<Result<ReviewResponseDto>>;
