using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.BusinessModul.Dto;

public record BusinessListtDto(
    Guid Id,
    Guid OwnerId,
    string? Owner,
    string Name,
    string Description,
    Guid CategoryId,
    string CategoryName,

    string? PrimaryImage,

    BusinessStatus Status,
    decimal AverageRating,
    int TotalReviews
);

public record BusinessListWithBranchsDto(
    Guid Id,
    Guid OwnerId,
    string? Owner,
    string Name,
    string Description,
    Guid CategoryId,
    string CategoryName,

    string? PrimaryImage,

    BusinessStatus Status,
    decimal AverageRating,
    int TotalReviews,

    List<MediaAssetDto> Images,

    List<BranchListDto> Branches
);
public record BranchListDto(
    Guid Id,
    Guid ManagerId,
    string? Manager,
    string Address,
    string City,
    string Country,
    string? Phone,
    decimal? AverageRating,
    int? TotalReviews,

    string? PrimaryImage
);

public record BusinessWithReviewDto
(
   Guid Id,
   string Name,

   string? PrimaryImage,

   decimal AverageRating,
   int TotalReviews,

   List<MediaAssetDto> Images,

   List<ReviewDto> Reviews
);

public record BranchWithReviewsDto(
    Guid Id,
    Guid ManagerId,
    string? Manager,
    string Address,
    string City,
    string Country,
    string? Phone,

    string? PrimaryImage,

    decimal AverageRating,
    int TotalReviews,

    List<MediaAssetDto> Images,

    List<ReviewDto> Reviews
);

public record ReviewDto(
    string UserName,
    string Comment,
    decimal OverallRating,
    decimal TrustScoreValue,
    string Status
);

public sealed record BusinessRatingDto(Guid BusinessId, decimal Avg, int Count);

public record MediaAssetDto(
    Guid Id,
    string Url,
    bool IsPrimary,
    int SortOrder
);
