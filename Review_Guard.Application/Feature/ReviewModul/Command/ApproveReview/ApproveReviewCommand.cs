using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Dto;
namespace Review_Guard.Application.Feature.ReviewModul.Command.ApproveReview;
public sealed record ApproveReviewCommand(Guid AdminId, Guid ReviewId, AdminReviewActionRequest Request) : IRequest<Result>;
