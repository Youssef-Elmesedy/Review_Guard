using Review_Guard.Application.Feature.UserModul.Dto;
using Review_Guard.Domain.Entities;
using System.Linq.Expressions;

namespace Review_Guard.Application.Feature.UserModul.Mapping;

public static class GetUserProjection
{
    public static Expression<Func<User, UserProfileResponse>> UserProfile =>
        u => new UserProfileResponse(
            u.Id,
            u.FullName,
            u.Email,
            u.IsEmailVerified,
            u.ProfileImageUrl,
            u.CreatedAt,
            0,   // TotalReviews   — enriched in service
            0,   // AverageRating
            0,   // ApprovedReviews
            0,   // RejectedReviews
            u.TrustScoreValue,
            u.Level.ToString()
        );

    public static Expression<Func<User, UserListItemDto>> UserListItem =>
        u => new UserListItemDto(
            u.Id,
            u.FullName,
            u.Email,
            u.Role.ToString(),
            u.Status.ToString(),
            u.TrustScoreValue,
            u.Level.ToString(),
            u.TotalReviewCount,
            u.CreatedAt,
            u.ProfileImageUrl
        );

    public static Expression<Func<UserActivity, UserActivityDto>> Activity =>
        a => new UserActivityDto(
            a.Id,
            a.Type.ToString(),      // ActivityType enum → string
            a.Description,
            a.CreatedAt
        );
}
