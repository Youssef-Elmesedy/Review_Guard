using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.Services;

namespace Review_Guard.Application.Feature.Auth.Queries.Logout;

internal sealed class LogoutQueryHandler : IRequestHandler<LogoutQuery, Result<string>>
{
    private readonly IAuthService _authService;


    public LogoutQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<string>> Handle(LogoutQuery request, CancellationToken ct = default)
    {
        var result = await _authService.LogoutAsync(ct);

        return result;
    }
}
