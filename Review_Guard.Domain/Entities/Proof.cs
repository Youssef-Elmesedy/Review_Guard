using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Entities;

public class Proof : BaseEntity
{
    private Proof() { }

    public Guid UserId { get; private set; }
    public Guid BranchId { get; private set; }
    public ProofType Type { get; private set; }
    public ProofStatus Status { get; private set; } = ProofStatus.Pending;

    public string? FileUrl { get; private set; }
    public string? OrderId { get; private set; }

    public string? AdminNote { get; private set; }
    public Guid? VerifiedByAdminId { get; private set; }
    public DateTime? VerifiedAt { get; private set; }

    // Navigation
    public User User { get; private set; } = default!;
    public Branch Branch { get; private set; } = default!;


    // ── Factory Methods ───────────────────────
    public static Proof CreateFromFile(Guid userId, Guid branchId, string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) throw new DomainException(
            DomainMessagies.ProofFileUrlRequired);

        return new Proof
        {
            UserId = userId,
            BranchId = branchId,
            Type = ProofType.Invoice,
            FileUrl = fileUrl
        };
    }

    public static Proof CreateFromOrder(Guid userId, Guid branchId, string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId)) throw new DomainException(DomainMessagies.ProofOrderIdRequired);

        return new Proof
        {
            UserId = userId,
            BranchId = branchId,
            Type = ProofType.Order,
            OrderId = orderId
        };
    }

    // ── Admin Actions ───────────────────────
    public void Verify(Guid adminId, string? note = null)
    {
        if (Status == ProofStatus.Verified) throw new DomainException(DomainMessagies.AlreadyVerified);

        Status = ProofStatus.Verified;
        VerifiedByAdminId = adminId;
        VerifiedAt = DateTime.UtcNow;
        AdminNote = note;
        SetUpdatedAt();
    }

    public void Reject(Guid adminId, string reason)
    {
        Status = ProofStatus.Rejected;
        VerifiedByAdminId = adminId;
        VerifiedAt = DateTime.UtcNow;
        AdminNote = reason;
        SetUpdatedAt();
    }
}