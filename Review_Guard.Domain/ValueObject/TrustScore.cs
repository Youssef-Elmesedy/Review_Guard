
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.ValueObject;

/// <summary>
/// Represents a user's trustworthiness score (0–100).
/// Derives the UserLevel classification which drives review submission rules:
///   LowTrust  (0–39)  → proof required + admin approval always
///   Normal    (40–69) → proof required, approval depends on RiskScore
///   Trusted   (70–100)→ auto-approved when RiskScore is low
/// </summary>
public sealed class TrustScore : ValueObject
{
    public const decimal MinValue = 0m;
    public const decimal MaxValue = 100m;
    public const decimal DefaultValue = 10m;

    // Level thresholds
    public const decimal LowTrustUpperBound = 39m;  // 0–39  → LowTrust
    public const decimal NormalUpperBound = 69m;  // 40–69 → Normal
    // 70–100 → Trusted

    // Behavioural thresholds
    public const decimal AutoApproveThreshold = 70m;
    public const decimal HighRiskThreshold = 30m;

    // Score adjustment constants
    public const decimal ApprovalBonus = 2m;
    public const decimal RejectionPenalty = 5m;
    public const decimal SubmissionBonus = 1m;
    public const decimal ReportConfirmedPenalty = 10m;

    public decimal Value { get; }

    private TrustScore(decimal value) => Value = value;

    public static TrustScore Create(decimal value)
    {
        if (value < MinValue || value > MaxValue)
            throw new DomainException(DomainMessagies.TrustScoreInvalid);

        return new TrustScore(Math.Round(value, 2));
    }

    public static TrustScore Default() => new(DefaultValue);

    // ── Derived Level ─────────────────────────────────────────────────────────
    /// <summary>
    /// Derives the UserLevel from the current score.
    /// This is the single source of truth — never store UserLevel separately.
    /// </summary>
    public UserLevel Level => Value switch
    {
        <= LowTrustUpperBound => UserLevel.LowTrust,
        <= NormalUpperBound => UserLevel.Normal,
        _ => UserLevel.Trusted
    };

    // ── Behavioural flags (used by domain rules) ──────────────────────────────
    public bool RequiresProof => Level != UserLevel.Trusted;
    public bool RequiresAdminApproval => Level == UserLevel.LowTrust;
    public bool CanAutoApproveOnLowRisk => Level == UserLevel.Trusted;
    public bool IsHighRisk => Value <= HighRiskThreshold;

    // ── Immutable mutation helpers ────────────────────────────────────────────
    public TrustScore Increase(decimal amount) =>
        new(Math.Min(MaxValue, Math.Round(Value + amount, 2)));

    public TrustScore Decrease(decimal amount) =>
        new(Math.Max(MinValue, Math.Round(Value - amount, 2)));

    protected override IEnumerable<object> GetAtomicValues() { yield return Value; }

    public override string ToString() => $"\n {Value:F1} ({Level})\n";
}
