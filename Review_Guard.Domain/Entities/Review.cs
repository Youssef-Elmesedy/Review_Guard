using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Entities;

public class Review : BaseEntity
{
    private Review() { }

    public Guid UserId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid? ProofId { get; private set; }

    // Multi Rating 
    public decimal FoodRating { get; private set; }
    public decimal ServiceRating { get; private set; }
    public decimal CleanlinessRating { get; private set; }
    public decimal AmbienceRating { get; private set; }
    public decimal ValueRating { get; private set; } = 0;

    public decimal OverallRating { get; private set; }
    public string Title { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public ReviewStatus Status { get; private set; } = ReviewStatus.Pending;

    public string? AdminNote { get; private set; }
    public Guid? ReviewedByAdminId { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    // Navigation
    public User User { get; private set; } = default!;
    public Branch Branch { get; private set; } = default!;
    public Proof? Proof { get; private set; }

    public static Review Create(
    Guid userId,
    Guid branchId,
    decimal foodRating,
    decimal serviceRating,
    decimal cleanlinessRating,
    decimal ambienceRating,
    decimal valueRating,
    string title,
    string content,
    Guid? proofId = null)
    {
        ValidateRating(foodRating);
        ValidateRating(serviceRating);
        ValidateRating(cleanlinessRating);
        ValidateRating(ambienceRating);
        ValidateRating(valueRating);

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required.",
                DomainMessagies.TitleIsRequired);

        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Content is required.",
                DomainMessagies.ContentIsRequired);

        if (content.Length < 20)
            throw new DomainException("Content must be at least 20 characters.",
                DomainMessagies.ContentIsRequiredLessThan20);

        var overall = CalculateOverall(
            foodRating,
            serviceRating,
            cleanlinessRating,
            ambienceRating,
            valueRating);

        return new Review
        {
            UserId = userId,
            BranchId = branchId,
            FoodRating = foodRating,
            ServiceRating = serviceRating,
            CleanlinessRating = cleanlinessRating,
            AmbienceRating = ambienceRating,
            ValueRating = valueRating,
            OverallRating = overall,
            Title = title.Trim(),
            Content = content.Trim(),
            ProofId = proofId
        };
    }

    private static void ValidateRating(decimal rating)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5.",
                DomainMessagies.InvalidRatingValue);
    }

    private static decimal CalculateOverall(
        decimal food,
        decimal service,
        decimal cleanliness,
        decimal ambience,
        decimal value)
    {
        return Math.Round((food + service + cleanliness + ambience + value) / 5.0m, 1);
    }

    public void Approve(Guid adminId, string? note = null)
    {
        Status = ReviewStatus.Approved;
        ReviewedByAdminId = adminId;
        ReviewedAt = DateTime.UtcNow;
        AdminNote = note;
        SetUpdatedAt();
    }

    public void Reject(Guid adminId, string reason)
    {
        Status = ReviewStatus.Rejected;
        ReviewedByAdminId = adminId;
        ReviewedAt = DateTime.UtcNow;
        AdminNote = reason;
        SetUpdatedAt();
    }


}
