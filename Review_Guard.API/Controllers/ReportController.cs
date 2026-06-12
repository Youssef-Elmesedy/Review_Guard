using Review_Guard.Application.Abstractions.Services.CurrentUserService;
using Review_Guard.Application.Feature.ReportModul.Command.CreateReport;
using Review_Guard.Application.Feature.ReportModul.Command.DismissReport;
using Review_Guard.Application.Feature.ReportModul.Command.ResolveReport;
using Review_Guard.Application.Feature.ReportModul.Dto;
using Review_Guard.Application.Feature.ReportModul.Query.GetAllReports;
using Review_Guard.Application.Feature.ReportModul.Query.GetReportById;

namespace Review_Guard.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public sealed class ReportController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ReportController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>[Admin] Get a single report by ID.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet("{reportId:guid}")]
    public async Task<IActionResult> GetById(Guid reportId, CancellationToken ct)
        => HandleResult(await _mediator.Send(new GetReportByIdQuery(reportId), ct));

    /// <summary>[Admin] Get all reports (paginated). Use openOnly=true to filter open reports.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
        [FromQuery] bool openOnly = false, CancellationToken ct = default)
        => HandleResult(await _mediator.Send(new GetAllReportsQuery(pageNumber, pageSize, openOnly), ct));

    /// <summary>Report a review for violating community guidelines.</summary>
    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReportRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new CreateReportCommand(_currentUser.UserId!.Value, request), ct));

    /// <summary>[Admin] Resolve a report (action taken).</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{reportId:guid}/resolve")]
    public async Task<IActionResult> Resolve(
        Guid reportId, [FromBody] AdminReportActionRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new ResolveReportCommand(_currentUser.AdminId!.Value, reportId, request), ct));

    /// <summary>[Admin] Dismiss a report (no action needed).</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{reportId:guid}/dismiss")]
    public async Task<IActionResult> Dismiss(
        Guid reportId, [FromBody] AdminReportActionRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new DismissReportCommand(_currentUser.AdminId!.Value, reportId, request), ct));
}
