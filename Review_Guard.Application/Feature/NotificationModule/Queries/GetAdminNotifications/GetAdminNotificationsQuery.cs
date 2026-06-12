using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.NotificationModule.DTOs;

namespace Review_Guard.Application.Feature.NotificationModule.Queries.GetAdminNotifications;

public sealed record GetAdminNotificationsQuery(
    Guid AdminId,
    bool UnreadOnly,
    int  PageNumber,
    int  PageSize
) : IRequest<Result<PagedResult<NotificationDto>>>;
