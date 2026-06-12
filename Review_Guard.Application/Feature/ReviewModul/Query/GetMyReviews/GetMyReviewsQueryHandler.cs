using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Dto;
using Review_Guard.Application.Feature.ReviewModul.Services;
namespace Review_Guard.Application.Feature.ReviewModul.Query.GetMyReviews;
internal sealed class GetMyReviewsQueryHandler : IRequestHandler<GetMyReviewsQuery, Result<PagedResult<ReviewListItemDto>>>
{
    private readonly IReadReviewService _service;
    public GetMyReviewsQueryHandler(IReadReviewService service) => _service = service;
    public async Task<Result<PagedResult<ReviewListItemDto>>> Handle(GetMyReviewsQuery request, CancellationToken ct)
        => await _service.GetMyReviewsAsync(request.UserId, new PaginationParams { PageNumber = request.PageNumber, PageSize = request.PageSize }, ct);
}
