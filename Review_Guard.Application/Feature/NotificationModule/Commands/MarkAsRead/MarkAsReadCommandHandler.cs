using MediatR;
using Review_Guard.Application.Abstractions.Repositories.NotificationRepository;
using Review_Guard.Application.Abstractions.UnitOfWork;
using Review_Guard.Application.Common;

namespace Review_Guard.Application.Feature.NotificationModule.Commands.MarkAsRead;

internal sealed class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Result>
{
    private readonly IReadNotificationRepository _readRepo;
    private readonly IWriteNotificationRepository _writeRepo;
    private readonly IUnitOfWork _uow;

    public MarkAsReadCommandHandler(
        IReadNotificationRepository readRepo,
        IWriteNotificationRepository writeRepo,
        IUnitOfWork uow)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _uow = uow;
    }

    public async Task<Result> Handle(MarkAsReadCommand request, CancellationToken ct)
    {
        var notification = await _readRepo.GetByIdAsync(request.NotificationId, ct);

        if (notification is null)
            return Result.Failure(AppErrorsCataloge.NotFound("Notification not found."));

        notification.MarkAsRead();
        await _writeRepo.UpdateAsync(notification, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}
