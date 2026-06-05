using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth.DTOs.Requests;
using Review_Guard.Application.Feature.Auth.DTOs.Responses;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.Auth.Services;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterUserAsync(RegisterUserDto request, CancellationToken ct);

    Task<Result<AuthResponseDto>> LoginUserAsync(LoginDto request, CancellationToken ct);

    Task<Result<AuthResponseDto>> LoginAdminAsync(LoginDto request, CancellationToken ct);

    Task<Result<string>> LogoutAsync(CancellationToken ct);

    Task<Result<AuthResponseDto>> RefreshTokenAsync(CancellationToken ct);

    Task<Result<MessageResponseDto>> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken ct);

    Task<Result<string>> VerifyResetCodeAsync(string code, CancellationToken ct);

    Task<Result<MessageResponseDto>> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct);

    Task VerifyEmailAsync(string code, CancellationToken ct);

    Task<Result<bool>> ResendVerificationCodeAsync(string email, string passwored, VerificationCodeType type, CancellationToken ct = default);
}

