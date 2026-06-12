using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Services;
namespace Review_Guard.Application.Feature.ReviewModul.Command.ApproveReview;
internal sealed class ApproveReviewCommandHandler : IRequestHandler<ApproveReviewCommand, Result>
{
    private readonly IWriteReviewService _service;
    public ApproveReviewCommandHandler(IWriteReviewService service) => _service = service;
    public async Task<Result> Handle(ApproveReviewCommand request, CancellationToken ct)
        => await _service.ApproveAsync(request.AdminId, request.ReviewId, request.Request, ct);
}
