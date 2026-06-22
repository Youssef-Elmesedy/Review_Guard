using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Entities;

public class Admin : BaseEntity
{
    private Admin() { }

    public string FullName { get; private set; } = string.Empty;
    public string NormalizedFullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public Roles Role { get; private set; } = default!;
    public int TotalActionsPerformed { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    //Navigation
    private readonly List<UserActivity> _activities = new();
    public IReadOnlyCollection<UserActivity> Activities => _activities.AsReadOnly();

    public static Admin Create(string fullName, string email,
        string passwordHash, Roles role)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException(DomainMessagies.EmailRequired);

        return new Admin
        {
            FullName = fullName.Trim(),
            NormalizedFullName = fullName.Trim().ToUpperInvariant(),
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash,
            Role = role,
        };
    }

    /// <summary>Updates editable profile fields for the admin themselves.</summary>
    public void UpdateProfile(string? fullName)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            FullName = fullName.Trim();
            NormalizedFullName = fullName.Trim().ToUpperInvariant();
        }
        SetUpdatedAt();
    }
    /// <summary>Changes the admin's password. The new password must be provided as a hash.</summary>
    public void ChangePassword(string newPasswordHash)
    {

        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException(
                DomainMessagies.PasswordRequired);

        PasswordHash = newPasswordHash;

        SetUpdatedAt();
    }

    public void RecordAction() { TotalActionsPerformed++; }
    public void RecordLogin() { LastLoginAt = DateTime.UtcNow; }
    public void Deactivate() { IsActive = false; }
    public void Activate() { IsActive = true; }
}
