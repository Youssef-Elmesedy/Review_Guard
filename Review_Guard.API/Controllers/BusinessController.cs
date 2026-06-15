using Review_Guard.Application.Feature.BusinessModul.Common.Command;
using Review_Guard.Application.Feature.BusinessModul.Common.Command.ApproveBusiness;
using Review_Guard.Application.Feature.BusinessModul.Common.Command.DeleteBusiness;
using Review_Guard.Application.Feature.BusinessModul.Common.Command.RejectBusiness;
using Review_Guard.Application.Feature.BusinessModul.Common.Command.UpdateBusiness;
using Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetAllBusiness;
using Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetAllBusinessWithReview;
using Review_Guard.Application.Feature.BusinessModul.Common.Queries.GetByBranchIdWithReviews;
using Review_Guard.Application.Feature.BusinessModul.Common.Queries.SercheByBusinessName;
using Review_Guard.Application.Feature.BusinessModul.Dto;

namespace Review_Guard.API.Controllers;

[Route("api/[controller]")]
public class BusinessController : BaseController
{
    private readonly IMediator _mediator;

    public BusinessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get paginated list of all businesses for home page
    /// </summary>
    /// <param name="pageNumber">Page number (default is 1)</param>
    /// <param name="pageSize">Number of items per page (default is 10)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paged list of businesses</returns>
    [AllowAnonymous]
    [HttpGet("Business")]
    public async Task<IActionResult> GetAllBusiness(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetAllBusinessQuery(pageNumber, pageSize),
            ct);

        return HandleResult(result);
    }

    /// <summary>
    /// Get business with all branchs by business id
    /// </summary>
    /// <param name="businessId">The unique identifier of the business</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Business with all branchs</returns>
    [AllowAnonymous]
    [HttpGet("BusinessWithBranchs/{businessId}")]
    public async Task<IActionResult> GetAllBusinessWithBranchs(
        Guid businessId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetAllBusinessWithBranchsQuery(businessId),
            ct);

        return HandleResult(result);
    }

    /// <summary>
    /// Get businesses filtered by category id
    /// </summary>
    /// <param name="categoryId">Business category id</param>
    /// <param name="pageNumber">Page number (default is 1)</param>
    /// <param name="pageSize">Page size (default is 10)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paged list of businesses in the category</returns>
    [AllowAnonymous]
    [HttpGet("BusinessWithBranchsByCategory/{categoryId}")]
    public async Task<IActionResult> GetAllByCategoryId(
        Guid categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetAllBusinessWithBranchsByCategoryIdQuery(pageNumber, pageSize, categoryId),
            ct);

        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves a paginated list of businesses along with their associated reviews.
    /// </summary>
    /// <remarks>Use this endpoint to browse businesses and their reviews in a paginated format. The response
    /// includes only the businesses and reviews for the specified page.</remarks>
    /// <param name="pageNumber">The page number of results to retrieve. Must be greater than or equal to 1.</param>
    /// <param name="pageSize">The maximum number of businesses to include in a single page. Must be greater than 0.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An IActionResult containing the paginated list of businesses with their reviews, or an appropriate error
    /// response.</returns>
    [AllowAnonymous]
    [HttpGet("BusinessWithReview")]
    public async Task<IActionResult> GetAllBusinessWithReview(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetAllBusinessWithReviewQuery(pageNumber, pageSize),
            ct);

        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves a list of branches for a specific business, along with their associated reviews.
    /// </summary>
    /// <param name="branchId">The unique identifier of the branch for which to retrieve reviews.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An IActionResult containing the list of branches with their reviews, or an appropriate error response.</returns>
    [AllowAnonymous]
    [HttpGet("BranchWithReviews/{branchId}")]
    public async Task<IActionResult> GetAllBranchWithReview(
        Guid branchId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetByBranchIdWithReviewsQuery(branchId),
            ct);

        return HandleResult(result);
    }


    /// <summary>
    /// Searches for businesses by business name with pagination support.
    /// </summary>
    /// <param name="businessName">
    /// The business name or partial name used for searching.
    /// </param>
    /// <param name="pageNumber">
    /// The page number to retrieve.
    /// </param>
    /// <param name="pageSize">
    /// The number of items per page.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// Returns a paginated list of businesses matching the provided name.
    /// </returns>
    [AllowAnonymous]
    [HttpGet("SearchByBusinessName")]
    public async Task<IActionResult> SearchByBusinessName(
        [FromQuery] string businessName,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new SercheByBusinessNameQuery(pageNumber, pageSize, businessName),
            ct);

        return HandleResult(result);
    }

    /// <summary>
    /// Register a new business (starts as PendingApproval, owner only).
    /// </summary>
    [Authorize(Roles = "User, BusinessOwner")]
    [HttpPost("CreateBusiness")]
    public async Task<IActionResult> CreateBusiness(
        [FromBody] CreateBusinessResponse response,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new CreateBusinessCommand(response), ct);

        return HandleResult(result);
    }

    /// <summary>
    /// Update business name/description. Owner only.
    /// </summary>
    [Authorize(Roles = "BusinessOwner")]
    [HttpPut("{businessId:guid}")]
    public async Task<IActionResult> UpdateBusiness(
        Guid businessId,
        [FromBody] UpdateBusinessRequest request,
        CancellationToken ct = default)
    {
        var response = new UpdateBusinessResponse(
            businessId, request.Name!, request.Description!, Guid.Empty, Guid.Empty, default);

        var result = await _mediator.Send(new UpdateBusinessCommand(response), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete (deactivate) a business. Owner or Admin.
    /// </summary>
    [Authorize(Roles = "BusinessOwner,Admin,SuperAdmin")]
    [HttpDelete("{businessId:guid}")]
    public async Task<IActionResult> DeleteBusiness(Guid businessId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new DeleteBusinessCommand(businessId), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// [Admin] Approve a business pending approval.
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{businessId:guid}/approve")]
    public async Task<IActionResult> ApproveBusiness(
        Guid businessId, [FromBody] AdminBusinessActionRequest request, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ApproveBusinessCommand(businessId, request.Note), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// [Admin] Reject a business pending approval, with a reason.
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{businessId:guid}/reject")]
    public async Task<IActionResult> RejectBusiness(
        Guid businessId, [FromBody] AdminBusinessActionRequest request, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new RejectBusinessCommand(businessId, request.Note ?? string.Empty), ct);
        return HandleResult(result);
    }
}