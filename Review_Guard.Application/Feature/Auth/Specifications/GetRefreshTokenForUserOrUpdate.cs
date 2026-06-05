using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Feature.Auth.Specifications;

public sealed class GetRefreshTokenForUserOrUpdate : BaseSpecification<RefreshToken>
{
    public GetRefreshTokenForUserOrUpdate(Guid? userId,
    Guid? adminId,
    string? ipAddress)
    {
        AddCriteria(x => x.UserId == userId && x.AdminId == adminId && x.RevokedAtUtc == null &&
                x.ExpiresAtUtc > DateTime.UtcNow);

        EnableTracking();
    }
}
