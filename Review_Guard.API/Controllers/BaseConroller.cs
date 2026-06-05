using Review_Guard.API.Errors;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return HandleError(result);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return HandleError(result);
    }

    private IActionResult HandleError(Result result)
    {
        var error = result.Error!;

        var statusCode = HttpStatusResolver.Resolve(error.Type);

        return StatusCode(statusCode, new
        {
            success = false,
            errorCode = error.Code,
            message = error.Message
        });
    }

    protected void SetRefreshTokenCookie(string token, DateTime expiresAt)
    {
        Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt
        });
    }

    protected string? GetRefreshTokenCookie()
    {
        return Request.Cookies["refreshToken"];
    }

    protected void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete("refreshToken");
    }
}