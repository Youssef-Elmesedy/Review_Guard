using Review_Guard.Domain.Enums;

namespace Review_Guard.Domain.Entities;

public class VerificationCode
{
    public Guid Id { get; set; }
    public string Code { get; private set; } = default!;

    public VerificationCodeType Type { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public bool IsUsed { get; private set; }

    public DateTime? UsedAtUtc { get; private set; }

    // FK
    public Guid UserId { get; private set; }

    // Navigation
    public User User { get; private set; } = default!;

    public bool IsExpired => DateTime.UtcNow > ExpiresAtUtc;

    private VerificationCode() { }

    public static VerificationCode Create(
        Guid userId,
        VerificationCodeType type,
        int expiryMinutes = 10)
    {
        return new VerificationCode
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Code = GenerateCode(),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes),
            IsUsed = false
        };
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAtUtc = DateTime.UtcNow;
    }

    public void Refresh(int expiryMinutes, VerificationCodeType type)
    {
        Code = GenerateCode();

        Type = type;

        ExpiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

        IsUsed = false;

        UsedAtUtc = null;
    }

    public static string GenerateCode()
    {
        return Random.Shared
            .Next(100000, 999999)
            .ToString();
    }
}
