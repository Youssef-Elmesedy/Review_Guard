using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.Services;

namespace Review_Guard.Application.Feature.Auth.Command.VerifyResetCode;

internal sealed class VerifyResetCodeCommandHandler : IRequestHandler<VerifyResetCodeCommand, Result<string>>
{
    private readonly IAuthService _authService;

    public VerifyResetCodeCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<string>> Handle(VerifyResetCodeCommand request, CancellationToken ct = default)
    {
        var result = await _authService.VerifyResetCodeAsync(request.code, ct);

        return result;
    }
}
