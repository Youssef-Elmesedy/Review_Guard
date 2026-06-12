using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Dto;
using Review_Guard.Application.Feature.ReviewModul.Services;
namespace Review_Guard.Application.Feature.ReviewModul.Query.GetReviewById;
internal sealed class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, Result<ReviewResponseDto>>
{
    private readonly IReadReviewService _service;
    public GetReviewByIdQueryHandler(IReadReviewService service) => _service = service;
    public async Task<Result<ReviewResponseDto>> Handle(GetReviewByIdQuery request, CancellationToken ct)
        => await _service.GetByIdAsync(request.ReviewId, ct);
}
