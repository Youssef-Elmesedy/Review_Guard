using Review_Guard.Application.Abstractions.Services.CurrentUserService;
using System.Security.Claims;

namespace Review_Guard.API.Middleware;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private HttpContext? Context =>
        _httpContextAccessor.HttpContext;

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            if (IsAdmin)
                return null;

            var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return value is not null &&
                   Guid.TryParse(value, out var id)
                ? id
                : null;
        }
    }

    public Guid? AdminId
    {
        get
        {
            if (!IsAdmin)
                return null;

            var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return value is not null &&
                   Guid.TryParse(value, out var id)
                ? id
                : null;
        }
    }

    public string? Email =>
        User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin =>
        User?.IsInRole("Admin") == true ||
        User?.IsInRole("SuperAdmin") == true;

    public string? Role =>
        User?.FindFirst(ClaimTypes.Role)?.Value;

    public string? IpAddress
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;

            if (context is null)
                return null;

            var forwardedFor = context.Request.Headers["X-Forwarded-For"]
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(forwardedFor))
                return forwardedFor.Split(',')[0].Trim();

            return context.Connection.RemoteIpAddress?.ToString();
        }
    }

    public string? UserAgent =>
        _httpContextAccessor.HttpContext?
            .Request.Headers["UserError-Agent"]
            .FirstOrDefault();

    public string? RefreshToken =>
        Context?.Request.Cookies["refreshToken"];
}