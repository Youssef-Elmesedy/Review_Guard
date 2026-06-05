using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.DTOs.Responses;
using Review_Guard.Application.Feature.Auth.Services;

namespace Review_Guard.Application.Feature.Auth.Queries.RefreshToken;

internal sealed class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, Result<AuthResponseDto>>
{
    private readonly IAuthService _authService;

    public RefreshTokenQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<Result<AuthResponseDto>> Handle(RefreshTokenQuery request, CancellationToken ct = default)
    {
        var result = _authService.RefreshTokenAsync(ct);

        return result;
    }
}
