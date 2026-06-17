using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Entities;

public class MediaAsset : BaseEntity
{
    private MediaAsset() { }

    public string Url { get; private set; } = default!;

    // ── Owner polymorphism (only one is set at a time) ─────────────
    public Guid? BusinessId { get; private set; }
    public Business? Business { get; private set; }

    public Guid? BranchId { get; private set; }
    public Branch? Branch { get; private set; }

    public Guid? ProofId { get; private set; }
    public Proof? Proof { get; private set; }

    // ── Common fields ──────────────────────────────────────────────
    public MediaOwnerType OwnerType { get; private set; }
    public MediaType Type { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsPrimary { get; private set; }

    // ── Computed: canonical OwnerId regardless of type ─────────────
    public Guid OwnerId => OwnerType switch
    {
        MediaOwnerType.Business => BusinessId!.Value,
        MediaOwnerType.Branch => BranchId!.Value,
        MediaOwnerType.Proof => ProofId!.Value,
        _ => throw new InvalidOperationException("Unknown owner type")
    };

    // ── Factory Methods ────────────────────────────────────────────

    public static MediaAsset CreateForBusiness(Guid businessId, string url, int sortOrder = 0, bool isPrimary = false)
        => new()
        {
            BusinessId = businessId,
            Url = url,
            OwnerType = MediaOwnerType.Business,
            Type = MediaType.Business,
            SortOrder = sortOrder,
            IsPrimary = isPrimary
        };

    public static MediaAsset CreateForBranch(Guid branchId, string url, int sortOrder = 0, bool isPrimary = false)
        => new()
        {
            BranchId = branchId,
            Url = url,
            OwnerType = MediaOwnerType.Branch,
            Type = MediaType.Branch,
            SortOrder = sortOrder,
            IsPrimary = isPrimary
        };

    public static MediaAsset CreateForProof(Guid proofId, string url)
     => new()
     {
         ProofId = proofId,
         Url = url,
         OwnerType = MediaOwnerType.Proof,
         Type = MediaType.Business,
         SortOrder = 0,
         IsPrimary = false
     };

    // ── Generic factory driven by OwnerType ────────────────────────
    public static MediaAsset Create(
        Guid ownerId,
        MediaOwnerType ownerType,
        string url,
        int sortOrder = 0,
        bool isPrimary = false)
        => ownerType switch
        {
            MediaOwnerType.Business => CreateForBusiness(ownerId, url, sortOrder, isPrimary),
            MediaOwnerType.Branch => CreateForBranch(ownerId, url, sortOrder, isPrimary),
            MediaOwnerType.Proof => CreateForProof(ownerId, url),
            _ => throw new DomainException("Unknown owner type", "Media.UnknownOwnerType")
        };

    // ── Domain Actions ─────────────────────────────────────────────

    public void SetPrimary()
    {
        IsPrimary = true;
        SetUpdatedAt();
    }

    public void UnsetPrimary()
    {
        IsPrimary = false;
        SetUpdatedAt();
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        SetUpdatedAt();
    }
}
