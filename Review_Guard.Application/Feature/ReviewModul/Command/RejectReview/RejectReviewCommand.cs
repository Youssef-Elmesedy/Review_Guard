using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Dto;
namespace Review_Guard.Application.Feature.ReviewModul.Command.RejectReview;
public sealed record RejectReviewCommand(Guid AdminId, Guid ReviewId, AdminReviewActionRequest Request) : IRequest<Result>;
