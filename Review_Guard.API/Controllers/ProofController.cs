using Review_Guard.Application.Abstractions.Services.CurrentUserService;
using Review_Guard.Application.Feature.ProofModul.Command.RejectProof;
using Review_Guard.Application.Feature.ProofModul.Command.SubmitProofByFile;
using Review_Guard.Application.Feature.ProofModul.Command.SubmitProofByOrder;
using Review_Guard.Application.Feature.ProofModul.Command.VerifyProof;
using Review_Guard.Application.Feature.ProofModul.Dto;
using Review_Guard.Application.Feature.ProofModul.Query.GetMyProofs;
using Review_Guard.Application.Feature.ProofModul.Query.GetPendingProofs;
using Review_Guard.Application.Feature.ProofModul.Query.GetProofById;

namespace Review_Guard.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public sealed class ProofController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ProofController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>[Admin] Get a single proof by ID.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet("{proofId:guid}")]
    public async Task<IActionResult> GetById(Guid proofId, CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetProofByIdQuery(proofId), ct));

    /// <summary>Get the authenticated user's submitted proofs (paginated).</summary>
    [Authorize(Roles = "User")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMine(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => HandleResult(await _mediator.Send(new GetMyProofsQuery(_currentUser.UserId!.Value, pageNumber, pageSize), ct));

    /// <summary>[Admin] Get all proofs pending verification.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => HandleResult(await _mediator.Send(new GetPendingProofsQuery(pageNumber, pageSize), ct));

    /// <summary>Submit a proof of visit using an uploaded file (invoice/receipt).</summary>
    [Authorize(Roles = "User")]
    [HttpPost("file")]
    public async Task<IActionResult> SubmitByFile([FromBody] SubmitProofByFileRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new SubmitProofByFileCommand(_currentUser.UserId!.Value, request), ct));

    /// <summary>Submit a proof of visit using an order ID.</summary>
    [Authorize(Roles = "User")]
    [HttpPost("order")]
    public async Task<IActionResult> SubmitByOrder([FromBody] SubmitProofByOrderRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new SubmitProofByOrderCommand(_currentUser.UserId!.Value, request), ct));

    /// <summary>[Admin] Verify a proof of visit.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{proofId:guid}/verify")]
    public async Task<IActionResult> Verify(
        Guid proofId, [FromBody] AdminProofActionRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new VerifyProofCommand(_currentUser.AdminId!.Value, proofId, request), ct));

    /// <summary>[Admin] Reject a proof of visit with a reason.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{proofId:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid proofId, [FromBody] AdminProofActionRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new RejectProofCommand(_currentUser.AdminId!.Value, proofId, request), ct));
}
