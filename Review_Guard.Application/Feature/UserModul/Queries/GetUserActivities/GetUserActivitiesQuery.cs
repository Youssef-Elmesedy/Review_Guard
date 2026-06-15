using MediatR;

namespace Review_Guard.Application.Feature.UserModul.Queries.GetUserActivities;

public sealed record GetUserActivitiesQuery(Guid UserId, int PageNumber, int PageSize)
    : IRequest<Result<PagedResult<UserActivityDto>>>;
