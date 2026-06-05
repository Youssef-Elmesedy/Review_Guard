namespace Review_Guard.Application.Abstractions.Services.CurrentUserService;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? AdminId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    string? IpAddress { get; }
    string? UserAgent { get; }
    string? RefreshToken { get; }
}
