using MediatR;
using Review_Guard.Application.Feature.ReviewModul.Dto;
using Review_Guard.Application.Feature.ReviewModul.Services;
namespace Review_Guard.Application.Feature.ReviewModul.Command.SubmitReview;

internal sealed class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, Result<ReviewResponseDto>>
{
    private readonly IWriteReviewService _service;
    public SubmitReviewCommandHandler(IWriteReviewService service) => _service = service;
    public async Task<Result<ReviewResponseDto>> Handle(SubmitReviewCommand request, CancellationToken ct)
        => await _service.SubmitAsync(request.UserId, request.Request, ct);
}
