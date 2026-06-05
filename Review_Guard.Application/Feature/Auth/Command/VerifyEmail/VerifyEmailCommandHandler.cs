using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.Services;

namespace Review_Guard.Application.Feature.Auth.Command.VerifyEmail;

public sealed class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IAuthService _authService;

    public VerifyEmailCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken ct = default)
    {
        await _authService.VerifyEmailAsync(request.Code, ct);

        return Result.Success();
    }
}
