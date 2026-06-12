using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.Dto;

namespace Review_Guard.Application.Feature.UserModul.Command.Query.GetUserActivities;

public sealed record GetUserActivitiesQuery(Guid UserId, int PageNumber, int PageSize)
    : IRequest<Result<PagedResult<UserActivityDto>>>;
