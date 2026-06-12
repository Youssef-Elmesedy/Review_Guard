using MediatR;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.NotificationModule.Commands.MarkAsRead;

public sealed record MarkAsReadCommand(Guid NotificationId) : IRequest<Result>;
