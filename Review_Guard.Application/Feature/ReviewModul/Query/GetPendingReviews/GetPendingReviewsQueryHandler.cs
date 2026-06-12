using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReviewModul.Dto;
using Review_Guard.Application.Feature.ReviewModul.Services;
namespace Review_Guard.Application.Feature.ReviewModul.Query.GetPendingReviews;
internal sealed class GetPendingReviewsQueryHandler : IRequestHandler<GetPendingReviewsQuery, Result<PagedResult<ReviewListItemDto>>>
{
    private readonly IReadReviewService _service;
    public GetPendingReviewsQueryHandler(IReadReviewService service) => _service = service;
    public async Task<Result<PagedResult<ReviewListItemDto>>> Handle(GetPendingReviewsQuery request, CancellationToken ct)
        => await _service.GetPendingReviewsAsync(new PaginationParams { PageNumber = request.PageNumber, PageSize = request.PageSize }, ct);
}
