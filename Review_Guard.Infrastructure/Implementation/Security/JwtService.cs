using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Review_Guard.Infrastructure.Implementation.Security;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> settings) => _settings = settings.Value;

    public string GenerateUserToken(User user)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email,          user.Email),
        new Claim(ClaimTypes.Name,           user.FullName),
        new Claim("trust_score",             user.TrustScoreValue.ToString("F1")),
        new Claim("IsEmailVerified",         user.IsEmailVerified.ToString()),
        new Claim(ClaimTypes.Role,           user.Role.ToString())
    };
        return BuildToken(claims, _settings.AccessTokenExpiryMinutes);
    }

    public string GenerateAdminToken(Admin admin)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
        new Claim(ClaimTypes.Email,          admin.Email),
        new Claim(ClaimTypes.Name,           admin.FullName),
        new Claim(ClaimTypes.Role,           admin.Role.ToString() )
    };
        return BuildToken(claims, _settings.AdminAccessTokenExpiryMinutes);
    }

    public Guid? ValidateTokenAndGetUserId(string token)
    {
        var principal = GetPrincipal(token);
        var claim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return claim is not null && Guid.TryParse(claim, out var id) ? id : null;
    }

    public bool IsTokenValid(string token) => GetPrincipal(token) is not null;

    private string BuildToken(IEnumerable<Claim> claims, int expiryMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            _settings.Issuer, _settings.Audience, claims,
            expires: expires, signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal? GetPrincipal(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.Secret);
            return handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ClockSkew = TimeSpan.Zero
            }, out _);
        }
        catch { return null; }
    }
}
