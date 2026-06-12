using MediatR;
using Review_Guard.Application.Common.ResultPattern;
namespace Review_Guard.Application.Feature.ReviewModul.Command.DeleteReview;
public sealed record DeleteReviewCommand(Guid CallerId, bool IsAdmin, Guid ReviewId) : IRequest<Result>;
