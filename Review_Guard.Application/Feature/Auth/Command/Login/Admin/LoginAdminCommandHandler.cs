using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.DTOs.Responses;
using Review_Guard.Application.Feature.Auth.Services;

namespace Review_Guard.Application.Feature.Auth.Command.Login.Admin;

internal sealed class LoginAdminCommandHandler : IRequestHandler<LoginAdminCommand, Result<AuthResponseDto>>
{
    private readonly IAuthService _authService;

    public LoginAdminCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginAdminCommand request, CancellationToken ct = default)
    {
        var result = await _authService.LoginAdminAsync(request.LoginDto, ct);

        return result;
    }
}
