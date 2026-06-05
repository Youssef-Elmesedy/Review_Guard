using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.DTOs.Responses;
using Review_Guard.Application.Feature.Auth.Services;

namespace Review_Guard.Application.Feature.Auth.Command.ForgotPassword;

internal sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<MessageResponseDto>>
{
    public readonly IAuthService _authService;

    public ForgotPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<MessageResponseDto>> Handle(ForgotPasswordCommand request, CancellationToken ct = default)
    {
        var result = await _authService.ForgotPasswordAsync(request.ForgotPasswordDto, ct);

        return result;
    }
}
