using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Entities;

public class Admin : BaseEntity
{
    private Admin() { }

    public string FullName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public Roles Role { get; private set; } = default!;
    public int TotalActionsPerformed { get; private set; }
    public DateTime? LastLoginAt { get; private set; }


    public static Admin Create(string fullName, string email,
        string passwordHash, Roles role)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Admin.EmailRequired", DomainMessagies.EmailRequired);

        return new Admin
        {
            FullName = fullName.Trim(),
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash,
            Role = role,
        };
    }

    //Navigation
    private readonly List<UserActivity> _activities = new();
    public IReadOnlyCollection<UserActivity> Activities => _activities.AsReadOnly();

    public void RecordAction() { TotalActionsPerformed++; }
    public void RecordLogin() { LastLoginAt = DateTime.UtcNow; }
    public void Deactivate() { IsActive = false; }
    public void Activate() { IsActive = true; }
}
