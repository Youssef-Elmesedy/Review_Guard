using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Services;
namespace Review_Guard.Application.Feature.ReviewModul.Command.DeleteReview;
internal sealed class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Result>
{
    private readonly IWriteReviewService _service;
    public DeleteReviewCommandHandler(IWriteReviewService service) => _service = service;
    public async Task<Result> Handle(DeleteReviewCommand request, CancellationToken ct)
        => await _service.DeleteAsync(request.CallerId, request.IsAdmin, request.ReviewId, ct);
}
