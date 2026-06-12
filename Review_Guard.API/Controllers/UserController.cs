using Review_Guard.Application.Abstractions.Services.CurrentUserService;
using Review_Guard.Application.Feature.UserModul.Command.BanUser;
using Review_Guard.Application.Feature.UserModul.Command.ChangePassword;
using Review_Guard.Application.Feature.UserModul.Command.Query;
using Review_Guard.Application.Feature.UserModul.Command.Query.GetAllUsers;
using Review_Guard.Application.Feature.UserModul.Command.Query.GetUserActivities;
using Review_Guard.Application.Feature.UserModul.Command.ReactivateUser;
using Review_Guard.Application.Feature.UserModul.Command.SuspendUser;
using Review_Guard.Application.Feature.UserModul.Command.UpdateImage;
using Review_Guard.Application.Feature.UserModul.Command.UpdateProfile;

namespace Review_Guard.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public sealed class UserController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public UserController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    // ══════════════════════════════════════════════════
    // READ
    // ══════════════════════════════════════════════════

    /// <summary>Get the authenticated user's own profile.</summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Unauthorized();

        return HandleResult(await _mediator.Send(new GetUserProfileQuery(userId.Value), ct));
    }

    /// <summary>[Admin] Get paginated list of all users.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
        => HandleResult(await _mediator.Send(new GetAllUsersQuery(pageNumber, pageSize), ct));

    /// <summary>Get activity log for a user. Owner or Admin.</summary>
    [HttpGet("{userId:guid}/activities")]
    public async Task<IActionResult> GetActivities(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => HandleResult(await _mediator.Send(new GetUserActivitiesQuery(userId, pageNumber, pageSize), ct));

    // ══════════════════════════════════════════════════
    // WRITE — self-service
    // ══════════════════════════════════════════════════

    /// <summary>Update the authenticated user's profile (name / profile image).</summary>
    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(new UpdateProfileCommand(userId.Value, request), ct));
    }

    /// <summary>Update the authenticated user's profile image.</summary>
    [HttpPatch("profile/image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateImage(
        [FromForm] UpdateImageRequest fileImage,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? _currentUser.AdminId;
        if (userId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(new UpdateImageCommand(userId.Value, fileImage.FileImage), ct));
    }

    /// <summary>Change the authenticated user's password.</summary>
    [HttpPatch("profile/change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(new ChangePasswordCommand(userId.Value, request), ct));
    }

    // ══════════════════════════════════════════════════
    // WRITE — Admin actions
    // ══════════════════════════════════════════════════

    /// <summary>[Admin] Suspend a user account.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPatch("{userId:guid}/suspend")]
    public async Task<IActionResult> Suspend(
        Guid userId,
        [FromBody] SuspendUserRequest request,
        CancellationToken ct)
    {
        var adminId = _currentUser.AdminId;
        if (adminId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(new SuspendUserCommand(adminId.Value, userId, request), ct));
    }

    /// <summary>[Admin] Permanently ban a user.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPatch("{userId:guid}/ban")]
    public async Task<IActionResult> Ban(
        Guid userId,
        [FromBody] BanUserRequest request,
        CancellationToken ct)
    {
        var adminId = _currentUser.AdminId;
        if (adminId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(new BanUserCommand(adminId.Value, userId, request), ct));
    }

    /// <summary>[Admin] Reactivate a suspended user.</summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPatch("{userId:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid userId, CancellationToken ct)
    {
        var adminId = _currentUser.AdminId;
        if (adminId is null) return Unauthorized();

        return HandleResult(await _mediator.Send(new ReactivateUserCommand(adminId.Value, userId), ct));
    }

    /// <summary>[SuperAdmin] Debug: show resolved client IP.</summary>
    [Authorize(Roles = "SuperAdmin")]
    [HttpGet("debug/ip")]
    public IActionResult GetIp() => Ok(new
    {
        remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
        forwarded = HttpContext.Request.Headers["X-Forwarded-For"].ToString()
    });
}
