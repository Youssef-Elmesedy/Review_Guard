using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Abstractions.Security;

public interface IJwtService
{
    string GenerateUserToken(User user);
    string GenerateAdminToken(Admin admin);
    Guid? ValidateTokenAndGetUserId(string token);
    bool IsTokenValid(string token);
}