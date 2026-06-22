// FILE: Review_Guard.Application / Feature / AdminModule / CQRS /
//       BroadcastNotification / BroadcastNotificationCommandHandler.cs

using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.AdminModule.Services;

namespace Review_Guard.Application.Feature.AdminModule.CQRS.BroadcastNotification;

internal sealed class BroadcastNotificationCommandHandler
    : IRequestHandler<BroadcastNotificationCommand, Result>
{
    private readonly IAdminService _service;

    public BroadcastNotificationCommandHandler(IAdminService service) => _service = service;

    public async Task<Result> Handle(
        BroadcastNotificationCommand request, CancellationToken ct)
        => await _service.BroadcastNotificationAsync(request.AdminId, request.Request, ct);
}
