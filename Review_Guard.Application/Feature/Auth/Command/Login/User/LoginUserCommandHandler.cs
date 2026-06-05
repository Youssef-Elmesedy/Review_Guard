using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.DTOs.Responses;
using Review_Guard.Application.Feature.Auth.Services;

namespace Review_Guard.Application.Feature.Auth.Command.Login.User;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<AuthResponseDto>>
{
    private readonly IAuthService _authService;

    public LoginUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginUserCommand request, CancellationToken ct = default)
    {
        var result = await _authService.LoginUserAsync(request.LoginDto, ct);

        return result;
    }
}
