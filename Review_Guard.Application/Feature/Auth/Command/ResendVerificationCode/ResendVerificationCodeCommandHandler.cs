using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.Services;

namespace Review_Guard.Application.Feature.Auth.Command.ResendVerificationCode;

public sealed class ResendVerificationCodeCommandHandler : IRequestHandler<ResendVerificationCodeCommand, Result>
{
    private readonly IAuthService _authService;

    public ResendVerificationCodeCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result> Handle(ResendVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.ResendVerificationCodeAsync(request.Email, request.password, request.Type, cancellationToken);

        return result;
    }
}
