namespace Review_Guard.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Token { get; private set; } = default!;

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public string CreatedByIp { get; private set; } = default!;

    public DateTime? RevokedAtUtc { get; private set; }

    public string? RevokedByIp { get; private set; }

    public string? ReplacedByToken { get; private set; }

    public string? RevokedReason { get; private set; }

    public Guid? UserId { get; private set; }
    public Guid? AdminId { get; private set; }

    public User? User { get; private set; }
    public Admin? Admin { get; private set; }

    // ── Helpers ─────────────────────────────

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    public bool IsActive => RevokedAtUtc is null && !IsExpired;

    public static RefreshToken Create(
        string token,
        DateTime expiresAtUtc,
        string ip,
        Guid? userId,
        Guid? adminId)
    {
        return new RefreshToken
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            CreatedByIp = ip,
            UserId = userId,
            AdminId = adminId
        };
    }

    public void Revoke(string ip, string reason)
    {
        RevokedAtUtc = DateTime.UtcNow;
        RevokedByIp = ip;
        RevokedReason = reason;
    }

    public void Replace(string newToken)
    {
        ReplacedByToken = newToken;
    }
}
