using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Services;
namespace Review_Guard.Application.Feature.ReviewModul.Command.RejectReview;
internal sealed class RejectReviewCommandHandler : IRequestHandler<RejectReviewCommand, Result>
{
    private readonly IWriteReviewService _service;
    public RejectReviewCommandHandler(IWriteReviewService service) => _service = service;
    public async Task<Result> Handle(RejectReviewCommand request, CancellationToken ct)
        => await _service.RejectAsync(request.AdminId, request.ReviewId, request.Request, ct);
}
