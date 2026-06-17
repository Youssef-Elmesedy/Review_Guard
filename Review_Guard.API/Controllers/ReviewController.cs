using Review_Guard.Application.Feature.ReviewModul.Command.ApproveReview;
using Review_Guard.Application.Feature.ReviewModul.Command.DeleteReview;
using Review_Guard.Application.Feature.ReviewModul.Command.RejectReview;
using Review_Guard.Application.Feature.ReviewModul.Command.SubmitReview;
using Review_Guard.Application.Feature.ReviewModul.Dto;
using Review_Guard.Application.Feature.ReviewModul.Query.GetMyReviews;
using Review_Guard.Application.Feature.ReviewModul.Query.GetPendingReviews;
using Review_Guard.Application.Feature.ReviewModul.Query.GetReviewById;

namespace Review_Guard.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public sealed class ReviewController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ReviewController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>Get a single review by ID.</summary>
    [AllowAnonymous]
    [HttpGet("{reviewId:guid}")]
    public async Task<IActionResult> GetById(Guid reviewId, CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetReviewByIdQuery(reviewId), ct));

    /// <summary>Get the authenticated user's reviews (paginated).</summary>
    [Authorize(Roles = "User")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMine(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => HandleResult(await _mediator.Send(new GetMyReviewsQuery(_currentUser.UserId!.Value, pageNumber, pageSize), ct));

    /// <summary>[Admin] Get all reviews pending moderation.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => HandleResult(await _mediator.Send(new GetPendingReviewsQuery(pageNumber, pageSize), ct));

    /// <summary>Submit a new review for a branch.</summary>
    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitReviewRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new SubmitReviewCommand(_currentUser.UserId!.Value, request), ct));

    /// <summary>[Admin] Approve a pending review.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{reviewId:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid reviewId, [FromBody] AdminReviewActionRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new ApproveReviewCommand(_currentUser.AdminId!.Value, reviewId, request), ct));

    /// <summary>[Admin] Reject a pending review with a reason.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{reviewId:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid reviewId, [FromBody] AdminReviewActionRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new RejectReviewCommand(_currentUser.AdminId!.Value, reviewId, request), ct));

    /// <summary>Delete a review. Owner can delete their own; Admin can delete any.</summary>
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> Delete(Guid reviewId, CancellationToken ct)
    {
        var isAdmin = _currentUser.IsAdmin;
        var callerId = isAdmin ? _currentUser.AdminId!.Value : _currentUser.UserId!.Value;
        return HandleResult(await _mediator.Send(new DeleteReviewCommand(callerId, isAdmin, reviewId), ct));
    }
}
