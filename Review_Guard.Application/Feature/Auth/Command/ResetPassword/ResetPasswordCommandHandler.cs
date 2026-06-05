using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.DTOs.Responses;
using Review_Guard.Application.Feature.Auth.Services;

namespace Review_Guard.Application.Feature.Auth.Command.ResetPassword;

internal sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<MessageResponseDto>>
{
    private readonly IAuthService _authService;

    public ResetPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<MessageResponseDto>> Handle(ResetPasswordCommand request, CancellationToken ct = default)
    {
        var result = await _authService.ResetPasswordAsync(request.ResetPasswordDto, ct);

        return result;
    }
}
