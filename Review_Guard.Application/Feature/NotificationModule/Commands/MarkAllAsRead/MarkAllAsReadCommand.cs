using MediatR;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.NotificationModule.Commands.MarkAllAsRead;

public sealed record MarkAllAsReadCommand(Guid RecipientId, bool IsAdmin) : IRequest<Result>;
