using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Events;
using Review_Guard.Domain.Exceptions;
using Review_Guard.Domain.ValueObject;

namespace Review_Guard.Domain.Entities;

public class User : BaseEntity
{
    // ── Core Info ─────────────────────────────────────────────
    public string FullName { get; private set; } = string.Empty;
    public string NormalizedFullName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Phone { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public Roles Role { get; private set; } = Roles.User;

    // ── Email Verification ───────────────────────────────────
    public bool IsEmailVerified { get; private set; }

    // ── Account Status ───────────────────────────────────────
    public AccountStatus Status { get; private set; } = AccountStatus.PendingVerification;
    public string? SuspensionReason { get; private set; }
    public DateTime? SuspendedUntil { get; private set; }

    // ── Trust Score ─────────────────────────────────────────
    public decimal TrustScoreValue { get; private set; } = TrustScore.DefaultValue;

    // ── Review Tracking ─────────────────────────────────────
    public int TotalReviewCount { get; private set; }
    public int ReviewsSubmittedToday { get; private set; }
    public DateTime? LastReviewSubmittedAt { get; private set; }
    public DateTime? LastDailyResetDate { get; private set; }

    // ── Misc ───────────────────────────────────────────────
    public string? ProfileImageUrl { get; private set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    // ── Navigation ─────────────────────────────────────────
    private readonly List<Review> _reviews = new();
    private readonly List<Business> _businesses = new();
    private readonly List<UserActivity> _activities = new();
    private readonly List<Proof> _proofs = new();
    private readonly List<UserReward> _rewards = new();
    private readonly List<VerificationCode> _verificationTokens = new();

    public IReadOnlyCollection<UserReward> Rewards => _rewards.AsReadOnly();
    public IReadOnlyCollection<Proof> Proofs => _proofs.AsReadOnly();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();
    public IReadOnlyCollection<Business> Businesses => _businesses.AsReadOnly();
    public IReadOnlyCollection<UserActivity> Activities => _activities.AsReadOnly();
    public IReadOnlyCollection<VerificationCode> VerificationTokens => _verificationTokens.AsReadOnly();

    // ── Computed / ValueObjects ────────────────────────────
    public TrustScore TrustScore => TrustScore.Create(TrustScoreValue);
    public UserLevel Level => TrustScore.Level;

    // Flags for readability
    public bool IsActive => Status == AccountStatus.Active;
    public bool IsSuspended => Status == AccountStatus.Suspended;
    public bool IsBanned => Status == AccountStatus.Banned;
    public bool CanSubmitReviews => IsActive && !TrustScore.RequiresProof;

    // ── Factory ────────────────────────────────────────────
    public static User Create(string fullName, string email, string passwordHash, Roles role = Roles.User)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException(DomainMessagies.FullNameRequired);
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException(DomainMessagies.EmailRequired);
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException(DomainMessagies.PasswordRequired);
        return new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            NormalizedFullName = fullName.Trim().ToUpperInvariant(),
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            IsEmailVerified = false,
            Status = AccountStatus.PendingVerification,
            CreatedAt = DateTime.UtcNow
        };
    }

    // ── Profile ────────────────────────────────────────────
    public bool UpdateProfile(
    string? fullName,
    string? description,
    string? phone)
    {
        var changed = false;

        if (!string.IsNullOrWhiteSpace(fullName) &&
            !string.Equals(FullName, fullName, StringComparison.Ordinal))
        {
            FullName = fullName.Trim();
            NormalizedFullName = fullName.Trim().ToUpperInvariant();
            changed = true;
        }

        if (!string.IsNullOrWhiteSpace(description) &&
            !string.Equals(Description, description, StringComparison.Ordinal))
        {
            Description = description;
            changed = true;
        }

        if (!string.IsNullOrWhiteSpace(phone) &&
            !string.Equals(Phone, phone, StringComparison.Ordinal))
        {
            Phone = phone;
            changed = true;
        }

        if (changed)
            SetUpdatedAt();

        return changed;
    }

    public void UpdateProfileImageUrl(string? url)
    {
        ProfileImageUrl = url;

        SetUpdatedAt();
    }
    // ── Role Management ────────────────────────────────────
    public void ChangeRole(Roles role)
    {
        Role = role;

        SetUpdatedAt();
    }

    public void ChangePassword(string newPasswordHash)
    {

        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException(
                DomainMessagies.PasswordRequired);

        PasswordHash = newPasswordHash;

        SetUpdatedAt();
    }

    // ── Email Verification ─────────────────────────────────
    public void VerifyEmail(string token)
    {
        if (IsEmailVerified)
            throw new DomainException(
                DomainMessagies.EmailAlreadyVerified);

        IsEmailVerified = true;

        Status = AccountStatus.Active;

        SetUpdatedAt();
    }



    // ── Account Status Management ─────────────────────────
    public void Suspend(string reason, DateTime? until = null)
    {
        if (IsBanned)
            throw new DomainException(
                DomainMessagies.AccountBanned);

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException(
                DomainMessagies.SuspensionReasonRequired);

        Status = AccountStatus.Suspended;
        SuspensionReason = reason;
        SuspendedUntil = until;

        SetUpdatedAt();
    }

    public void Ban(string reason)
    {
        if (IsBanned)
            throw new DomainException(
                DomainMessagies.AccountAlreadyBanned);

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException(
                DomainMessagies.BanReasonRequired);

        Status = AccountStatus.Banned;
        SuspensionReason = reason;
        SuspendedUntil = null;

        SetUpdatedAt();
    }

    public void Reactivate()
    {
        if (!IsSuspended)
            throw new DomainException(
                DomainMessagies.AccountNotSuspended);

        Status = AccountStatus.Active;
        SuspensionReason = null;
        SuspendedUntil = null;

        SetUpdatedAt();
    }

    public void CheckSuspensionExpiry()
    {
        if (IsSuspended && SuspendedUntil.HasValue && SuspendedUntil <= DateTime.UtcNow)
        {
            Reactivate();
        }
    }

    public void EnsureCanLogin()
    {
        CheckSuspensionExpiry();

        if (IsBanned)
            throw new DomainException(
                DomainMessagies.AccountBanned);

        if (IsSuspended)
            throw new DomainException(
                DomainMessagies.AccountSuspended);

        if (Status == AccountStatus.PendingVerification)
            throw new DomainException(DomainMessagies.EmailNotVerified);
    }

    // ── Trust Score Management ────────────────────────────
    public void IncreaseTrustScore(decimal amount)
    {
        TrustScoreValue = TrustScore.Increase(amount).Value;

        SetUpdatedAt();
    }

    public void DecreaseTrustScore(decimal amount)
    {
        TrustScoreValue = TrustScore.Decrease(amount).Value;
        SetUpdatedAt();
    }

    // ── Review Submission Tracking ───────────────────────
    public void ValidateCanSubmitReview(int maxReviewsPerDay)
    {
        EnsureCanLogin();
        ResetDailyReviewCountIfNeeded();

        if (ReviewsSubmittedToday >= maxReviewsPerDay)
            throw new DomainException(DomainMessagies.DailyReviewLimitExceeded);
    }

    public void RecordReviewSubmission()
    {
        ReviewsSubmittedToday++;
        TotalReviewCount++;
        LastReviewSubmittedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    private void ResetDailyReviewCountIfNeeded()
    {
        if (!LastDailyResetDate.HasValue || LastDailyResetDate.Value.Date < DateTime.UtcNow.Date)
        {
            ReviewsSubmittedToday = 0;
            LastDailyResetDate = DateTime.UtcNow.Date;
        }
    }

    //---------------Add Rewards-----------------------
    public void AddReward(RewardType rewardType)
    {
        if (_rewards.Any(r => r.Type == rewardType))
            return;

        var reward = UserReward.Create(Id, rewardType);

        _rewards.Add(reward);

        RaiseDomainEvent(new RewardAddedEvent(Id, reward));
    }

    //---------------Add EmailEvent-----------------------
    public void RaiseRegisteredEvent(string code)
    {
        RaiseDomainEvent(
            new UserRegisteredEvent(
                Id,
                Email,
                FullName,
                code
            )
        );
    }

}