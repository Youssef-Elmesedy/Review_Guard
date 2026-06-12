using MediatR;
using Review_Guard.Application.Abstractions.Repositories.NotificationRepository;
using Review_Guard.Application.Abstractions.UnitOfWork;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.NotificationModule.Commands.MarkAllAsRead;

internal sealed class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, Result>
{
    private readonly IWriteNotificationRepository _writeRepo;
    private readonly IUnitOfWork _uow;

    public MarkAllAsReadCommandHandler(IWriteNotificationRepository writeRepo, IUnitOfWork uow)
    {
        _writeRepo = writeRepo;
        _uow       = uow;
    }

    public async Task<Result> Handle(MarkAllAsReadCommand request, CancellationToken ct)
    {
        await _writeRepo.MarkAllAsReadAsync(request.RecipientId, request.IsAdmin, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
