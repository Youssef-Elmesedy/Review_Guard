namespace Review_Guard.Application.Feature.Auth.DTOs.Responses;

public record AuthResponseDto
(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string Role,
    Guid UserId,
    string Email
);