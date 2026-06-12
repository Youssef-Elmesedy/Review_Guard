using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.NotificationModule.DTOs;

namespace Review_Guard.Application.Feature.NotificationModule.Queries.GetMyNotifications;

public sealed record GetMyNotificationsQuery(
    Guid UserId,
    bool UnreadOnly,
    int  PageNumber,
    int  PageSize
) : IRequest<Result<PagedResult<NotificationDto>>>;
