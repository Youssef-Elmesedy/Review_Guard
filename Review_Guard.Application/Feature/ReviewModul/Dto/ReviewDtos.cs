using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.ReviewModul.Dto;

public sealed record SubmitReviewRequest(
    Guid businessId,
    Guid BranchId,
    decimal FoodRating,
    decimal ServiceRating,
    decimal CleanlinessRating,
    decimal AmbienceRating,
    decimal ValueRating,
    string Content,
    Guid ProofId
);

public sealed record AdminReviewActionRequest(string? Note);

public sealed record ReviewResponseDto(
    Guid Id,
    Guid UserId,
    string UserFullName,
    Guid BranchId,
    string BranchAddress,
    decimal FoodRating,
    decimal ServiceRating,
    decimal CleanlinessRating,
    decimal AmbienceRating,
    decimal ValueRating,
    decimal OverallRating,
    string Title,
    string Content,
    ReviewStatus Status,
    string? AdminNote,
    DateTime CreatedAt
);

public sealed record ReviewListItemDto(
    Guid Id,
    string UserFullName,
    string BranchAddress,
    decimal OverallRating,
    string Title,
    ReviewStatus Status,
    DateTime CreatedAt
);
