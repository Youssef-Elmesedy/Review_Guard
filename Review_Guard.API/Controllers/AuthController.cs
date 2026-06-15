namespace Review_Guard.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user account and returns authentication tokens.
    /// </summary>
    /// <param name="request">UserError registration data</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Authentication result with refresh token</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new RegistrationRegisterUserCommand(request), ct);

        if (result.IsSuccess)
            SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.ExpiresAt);

        return HandleResult(result);
    }

    /// <summary>
    /// Authenticates a normal user and returns JWT + refresh token.
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Authentication result</returns>
    [AllowAnonymous]
    [HttpPost("loginUser")]
    public async Task<IActionResult> LoginUser([FromBody] LoginDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginUserCommand(request), ct);

        if (result.IsSuccess)
            SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.ExpiresAt);

        return HandleResult(result);
    }

    /// <summary>
    /// Authenticates an admin or super admin user.
    /// </summary>
    /// <param name="request">Admin login credentials</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Authentication result</returns>
    [AllowAnonymous]
    [HttpPost("loginAdmin")]
    public async Task<IActionResult> LoginAdmin([FromBody] LoginDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginAdminCommand(request), ct);

        if (result.IsSuccess)
            SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.ExpiresAt);

        return HandleResult(result);
    }

    /// <summary>
    /// Logs out the current authenticated user or admin.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Logout result</returns>
    [Authorize(Roles = "User,BusinessOwner,Admin,SuperAdmin")]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var result = await _mediator.Send(new LogoutQuery(), ct);

        if (result.IsSuccess)
            DeleteRefreshTokenCookie();

        return HandleResult(result);
    }

    /// <summary>
    /// Generates a new access token using refresh token.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>New JWT and refresh token</returns>
    [Authorize(Roles = "User,BusinessOwner,Admin,SuperAdmin")]
    [HttpGet("refresh-token")]
    public async Task<IActionResult> RefreshToken(CancellationToken ct)
    {
        var result = await _mediator.Send(new RefreshTokenQuery(), ct);

        if (result.IsSuccess)
            SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.ExpiresAt);

        return HandleResult(result);
    }

    /// <summary>
    /// Sends a password reset request email.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("Forget-Password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgotPasswordDto request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new ForgotPasswordCommand(request), ct));

    /// <summary>
    /// Verifies password reset code sent to email.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("Verify-ResetCode")]
    public async Task<IActionResult> VerifyResetCode([FromQuery] string code, CancellationToken ct)
        => HandleResult(await _mediator.Send(new VerifyResetCodeCommand(code), ct));

    /// <summary>
    /// Resets user password using verified reset code.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("Reset-Password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new ResetPasswordCommand(request), ct));

    /// <summary>
    /// Verifies user email using verification code.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string code, CancellationToken ct)
        => HandleResult(await _mediator.Send(new VerifyEmailCommand(code), ct));

    /// <summary>
    /// Resends email verification or password reset code.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("Resend-Verification-Code")]
    public async Task<IActionResult> ResendVerificationCode(
        [FromQuery] string email,
        [FromQuery] string password,
        [FromQuery] VerificationCodeType type,
        CancellationToken ct)
        => HandleResult(await _mediator.Send(
            new ResendVerificationCodeCommand(email, password, type), ct));
}