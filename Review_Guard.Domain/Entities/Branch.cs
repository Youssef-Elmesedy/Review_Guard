using Review_Guard.Domain.Common;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Entities;

public class Branch : BaseEntity
{
    private Branch() { }

    public string Address { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string Country { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;

    public Guid BusinessId { get; private set; }
    public Business Business { get; private set; } = default!;

    public Guid ManagerId { get; private set; }
    public User Manager { get; private set; } = default!;

    public bool IsManagedBy(Guid userId) => ManagerId == userId;

    // Navigation 
    private readonly List<Review> _reviews = new();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    private readonly List<MediaAsset> _media = new();
    public IReadOnlyCollection<MediaAsset> Media => _media.AsReadOnly();


    // Upload branch image
    public void AddImage(string url, int sortOrder, bool isPrimary = false)
    {
        _media.Add(MediaAsset.CreateForBranch(Id, url, sortOrder, isPrimary));

        SetUpdatedAt();
    }
    public void SetPrimaryImage(Guid mediaId)
    {
        var media = _media.FirstOrDefault(x => x.Id == mediaId)
            ?? throw new DomainException(DomainMessagies.ImageNotFound);

        foreach (var img in _media)
            img.UnsetPrimary();

        media.SetPrimary();

        SetUpdatedAt();
    }

    // ── Rating Fields ─────────────────────────
    public decimal SimpleAverageRating { get; private set; }
    public decimal WeightedAverageRating { get; private set; }
    public int TotalReviews { get; private set; }
    public int ApprovedReviewCount { get; private set; }
    public int PendingReviewCount { get; private set; }

    // ── Factory ───────────────────────────────
    public static Branch Create(Guid businessId, string address, string city, string country, string phone, Guid managerId)
    {
        if (string.IsNullOrWhiteSpace(address)) throw new DomainException(DomainMessagies.AddressRequired);
        if (string.IsNullOrWhiteSpace(phone)) throw new DomainException(DomainMessagies.PhoneRequired);

        return new Branch
        {
            BusinessId = businessId,
            Address = address.Trim(),
            City = city.Trim(),
            Country = country.Trim(),
            PhoneNumber = phone.Trim(),
            ManagerId = managerId
        };
    }


    // ── Manager Actions ───────────────────────
    public void ChangeManager(Guid currentUserId, Guid ownerId, Guid newManagerId)
    {
        if (currentUserId != ownerId) throw new DomainException(DomainMessagies.Unauthorized);
        ManagerId = newManagerId;
        SetUpdatedAt();
    }

    // ── Weighted Rating Recalculation ─────────────────────────────────────────
    public void RecalculateRatings(
     IEnumerable<(decimal Rating, decimal TrustWeight)> approvedReviews,
     int pendingCount)
    {
        var list = approvedReviews.ToList();

        ApprovedReviewCount = list.Count;
        TotalReviews = ApprovedReviewCount + pendingCount;

        if (!list.Any())
        {
            SimpleAverageRating = 0;
            WeightedAverageRating = 0;
            SetUpdatedAt();
            return;
        }

        SimpleAverageRating = Math.Round(list.Average(r => r.Rating), 2);

        var totalWeight = list.Sum(r => r.TrustWeight);

        WeightedAverageRating = totalWeight == 0
            ? SimpleAverageRating
            : Math.Round(list.Sum(r => r.Rating * r.TrustWeight) / totalWeight, 2);

        SetUpdatedAt();
    }

    public void IncrementPendingReviews() { PendingReviewCount++; SetUpdatedAt(); }
    public void DecrementPendingReviews()
    {
        if (PendingReviewCount > 0) PendingReviewCount--;
        SetUpdatedAt();
    }
}