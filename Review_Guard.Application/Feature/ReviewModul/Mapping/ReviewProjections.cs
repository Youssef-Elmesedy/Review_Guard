using Review_Guard.Application.Feature.ReviewModul.Dto;
using Review_Guard.Domain.Entities;
using System.Linq.Expressions;

namespace Review_Guard.Application.Feature.ReviewModul.Mapping;

public static class ReviewProjections
{
    public static Expression<Func<Review, ReviewResponseDto>> Full =>
        r => new ReviewResponseDto(
            r.Id, r.UserId, r.User.FullName, r.BranchId, r.Branch.Address,
            r.FoodRating, r.ServiceRating, r.CleanlinessRating, r.AmbienceRating, r.ValueRating,
            r.OverallRating, r.Title, r.Content, r.Status, r.AdminNote, r.CreatedAt);

    public static Expression<Func<Review, ReviewListItemDto>> ListItem =>
        r => new ReviewListItemDto(
            r.Id, r.User.FullName, r.Branch.Address, r.OverallRating, r.Title, r.Status, r.CreatedAt);
}
