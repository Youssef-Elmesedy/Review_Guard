using Review_Guard.Application.Abstractions.Services.MediaService;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Domain.Entities;
using System.Linq.Expressions;

namespace Review_Guard.Application.Feature.BusinessModul.Mapping;

public static class BusinessProjections
{
    public static Expression<Func<Business, BusinessListWithBranchsDto>> BusinessWithBranchs =>
     b => new BusinessListWithBranchsDto(
         b.Id,
         b.OwnerId,
         b.Owner.FullName,
         b.Name,
         b.Description,
         b.BusinessCategoryId,
         b.BusinessCategory.Name,

         b.Media
             .Where(x => x.IsPrimary)
             .Select(x => x.Url)
             .FirstOrDefault(),

         b.Status,
         0,
         0,

         b.Media
             .OrderBy(x => x.SortOrder)
             .Select(x => new MediaAssetDto(
                 x.Id,
                 x.Url,
                 x.IsPrimary,
                 x.SortOrder,
                 x.CreatedAt
             )).ToList(),

         b.Branches.Select(br => new BranchListDto(
             br.Id,
             br.ManagerId,
             br.Manager.FullName,
             br.Address,
             br.City,
             br.Country,
             br.PhoneNumber,
             br.WeightedAverageRating,
                br.TotalReviews,

             br.Media
                 .Where(x => x.IsPrimary)
                 .Select(x => x.Url)
                 .FirstOrDefault()
         )).ToList()
     );

    public static Expression<Func<Business, BusinessListtDto>> BusinessList =>
     b => new BusinessListtDto(
         b.Id,
         b.OwnerId,
         b.Owner.FullName,
         b.Name,
         b.Description,
         b.BusinessCategoryId,
         b.BusinessCategory.Name,

         b.Media
             .Where(x => x.IsPrimary)
             .Select(x => x.Url)
             .FirstOrDefault(),

         b.Status,
         0,
         0
     );

    public static Expression<Func<Business, BusinessWithReviewDto>> BusinessDetails =>
    b => new BusinessWithReviewDto(
        b.Id,
        b.Name,

        b.Media
            .Where(x => x.IsPrimary)
            .Select(x => x.Url)
            .FirstOrDefault(),

        0,
        0,

        b.Media
            .OrderBy(x => x.SortOrder)
            .Select(x => new MediaAssetDto(
                x.Id,
                x.Url,
                x.IsPrimary,
                x.SortOrder,
                x.CreatedAt
            )).ToList(),

        b.Branches.SelectMany(br => br.Reviews.Select(r => new ReviewDto(
            r.User.FullName,
            r.Content,
            r.OverallRating,
            r.User.TrustScoreValue,
            r.Status.ToString()
        ))).ToList()
    );

    public static Expression<Func<Branch, BranchWithReviewsDto>> BrancheWithReviews =>
  b => new BranchWithReviewsDto(
      b.Id,
      b.ManagerId,
      b.Manager.FullName,
      b.Address,
      b.City,
      b.Country,
      b.PhoneNumber,

      b.Media
         .Where(x => x.IsPrimary)
         .Select(x => x.Url)
         .FirstOrDefault(),

      b.WeightedAverageRating,
      b.TotalReviews,

      b.Media
         .OrderBy(x => x.SortOrder)
         .Select(x => new MediaAssetDto(
             x.Id,
             x.Url,
             x.IsPrimary,
             x.SortOrder,
             x.CreatedAt
         )).ToList(),

      b.Reviews.Select(r => new ReviewDto(
          r.User.FullName,
          r.Content,
          r.OverallRating,
          r.User.TrustScoreValue,
          r.Status.ToString()
      )).ToList()
  );
}

