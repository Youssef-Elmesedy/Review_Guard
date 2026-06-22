// FILE: Review_Guard.Application / Feature / AdminModule / CQRS /
//       BroadcastNotification / BroadcastNotificationCommand.cs

using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.AdminModule.DTOs;

namespace Review_Guard.Application.Feature.AdminModule.CQRS.BroadcastNotification;

public sealed record BroadcastNotificationCommand(
    Guid AdminId,
    BroadcastNotificationRequest Request)
    : IRequest<Result>;
