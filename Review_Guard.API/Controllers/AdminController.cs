// FILE: Review_Guard.API / Controllers / AdminController.cs

using Review_Guard.Application.Feature.AdminModul.CQRS.ChangePassword;
using Review_Guard.Application.Feature.AdminModule.CQRS.BroadcastNotification;
using Review_Guard.Application.Feature.AdminModule.CQRS.GetAdminDashboard;
using Review_Guard.Application.Feature.AdminModule.CQRS.GetAdminProfile;
using Review_Guard.Application.Feature.AdminModule.CQRS.UpdateAdminProfile;
using Review_Guard.Application.Feature.AdminModule.DTOs;

namespace Review_Guard.API.Controllers;

/// <summary>
/// Admin self-management endpoints:
/// profile, dashboard statistics, profile update, and platform-wide broadcast.
/// </summary>
[Authorize(Roles = "Admin,SuperAdmin")]
[Route("api/[controller]")]
public sealed class AdminController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    // ══════════════════════════════════════════════════
    // READ
    // ══════════════════════════════════════════════════

    /// <summary>Get the authenticated admin's own profile.</summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var adminId = _currentUser.AdminId;
        if (adminId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(
            new GetAdminProfileQuery(adminId.Value), ct));
    }

    /// <summary>
    /// Get dashboard statistics: user counts, review counts,
    /// business counts, and open reports.
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken ct)
    {
        var adminId = _currentUser.AdminId;
        if (adminId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(
            new GetAdminDashboardQuery(adminId.Value), ct));
    }

    // ══════════════════════════════════════════════════
    // WRITE
    // ══════════════════════════════════════════════════

    /// <summary>Update the authenticated admin's profile (FullName only).</summary>
    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateAdminProfileRequest request,
        CancellationToken ct)
    {
        var adminId = _currentUser.AdminId;
        if (adminId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(
            new UpdateAdminProfileCommand(adminId.Value, request), ct));
    }

    /// <summary> change password endpoint is intentionally omitted to avoid complexity. </summary>
    [HttpPatch("password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangeAdminPasswordRequest request,
        CancellationToken ct)
    {
        var adminId = _currentUser.AdminId;
        if (adminId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(
            new ChangePasswordAdminCommand(adminId.Value, request), ct));
    }

    // ══════════════════════════════════════════════════
    // BROADCAST NOTIFICATION
    // ══════════════════════════════════════════════════

    /// <summary>
    /// [SuperAdmin] Broadcast a general notification to ALL active users.
    /// Use for maintenance windows, feature announcements, etc.
    /// </summary>
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast(
        [FromBody] BroadcastNotificationRequest request,
        CancellationToken ct)
    {
        var adminId = _currentUser.AdminId;
        if (adminId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(
            new BroadcastNotificationCommand(adminId.Value, request), ct));
    }
}
