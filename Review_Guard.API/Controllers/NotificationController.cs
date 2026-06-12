using Review_Guard.Application.Abstractions.Services.CurrentUserService;
using Review_Guard.Application.Feature.NotificationModule.Commands.MarkAllAsRead;
using Review_Guard.Application.Feature.NotificationModule.Commands.MarkAsRead;
using Review_Guard.Application.Feature.NotificationModule.Queries.GetAdminNotifications;
using Review_Guard.Application.Feature.NotificationModule.Queries.GetMyNotifications;

namespace Review_Guard.API.Controllers;

/// <summary>
/// Notification endpoints for Users, Business Owners and Admins.
/// </summary>
[Authorize]
[Route("api/[controller]")]
public sealed class NotificationController : BaseController
{
    private readonly IMediator           _mediator;
    private readonly ICurrentUserService _currentUser;

    public NotificationController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator    = mediator;
        _currentUser = currentUser;
    }

    // ══════════════════════════════════════════════════
    // USER / BUSINESS OWNER
    // ══════════════════════════════════════════════════

    /// <summary>
    /// Get notifications for the authenticated user (or business owner).
    /// Pass unreadOnly=true to fetch only unseen notifications.
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMine(
        [FromQuery] bool unreadOnly  = false,
        [FromQuery] int  pageNumber  = 1,
        [FromQuery] int  pageSize    = 20,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(
            new GetMyNotificationsQuery(userId.Value, unreadOnly, pageNumber, pageSize), ct));
    }

    /// <summary>Mark a single notification as read.</summary>
    [HttpPut("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId, CancellationToken ct)
        => HandleResult(await _mediator.Send(new MarkAsReadCommand(notificationId), ct));

    /// <summary>Mark ALL my notifications as read.</summary>
    [HttpPut("my/read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(new MarkAllAsReadCommand(userId.Value, false), ct));
    }

    // ══════════════════════════════════════════════════
    // ADMIN
    // ══════════════════════════════════════════════════

    /// <summary>[Admin] Get notifications sent to the authenticated admin.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet("admin")]
    public async Task<IActionResult> GetAdminNotifications(
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int  pageNumber = 1,
        [FromQuery] int  pageSize   = 20,
        CancellationToken ct = default)
    {
        var adminId = _currentUser.AdminId;
        if (adminId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(
            new GetAdminNotificationsQuery(adminId.Value, unreadOnly, pageNumber, pageSize), ct));
    }

    /// <summary>[Admin] Mark ALL admin notifications as read.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("admin/read-all")]
    public async Task<IActionResult> MarkAdminAllAsRead(CancellationToken ct)
    {
        var adminId = _currentUser.AdminId;
        if (adminId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(new MarkAllAsReadCommand(adminId.Value, true), ct));
    }
}
