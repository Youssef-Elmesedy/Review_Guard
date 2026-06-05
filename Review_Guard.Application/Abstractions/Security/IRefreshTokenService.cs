using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Abstractions.Security;

public interface IRefreshTokenService
{
    RefreshToken Generate(Guid? userId, Guid? adminId, string? ipAddress);

    Task<RefreshToken> GetOrCreateAsync(Guid? userId, Guid? adminId, string? ipAddress, CancellationToken ct = default);

    Task<RefreshToken?> GetAsync(string token, CancellationToken ct = default);

    Task RevokeAsync(RefreshToken token, string ipAddress, string reason, CancellationToken ct = default);

    Task<RefreshToken> RotateAsync(RefreshToken oldToken, string ipAddress, CancellationToken ct = default);

    Task ValidateOwnershipAsync(RefreshToken token, Guid? currentUserId, Guid? currentAdminId);
}