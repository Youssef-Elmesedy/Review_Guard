using Review_Guard.Domain.Enums;

namespace Review_Guard.Domain.Entities;

public class ActionToken
{
    public Guid Id { get; set; }
    public string Token { get; private set; } = default!;

    public TokenType Type { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public bool IsUsed { get; private set; }

    public Guid UserId { get; private set; }

    public User User { get; private set; } = default!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    private ActionToken() { }

    public static ActionToken Create(
        Guid userId,
        TokenType type,
        int expiryMinutes)
    {
        return new ActionToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }
}
