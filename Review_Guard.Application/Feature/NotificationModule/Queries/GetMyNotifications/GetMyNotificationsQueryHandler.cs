using MediatR;
using Review_Guard.Application.Abstractions.Repositories.NotificationRepository;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.NotificationModule.DTOs;

namespace Review_Guard.Application.Feature.NotificationModule.Queries.GetMyNotifications;

internal sealed class GetMyNotificationsQueryHandler
    : IRequestHandler<GetMyNotificationsQuery, Result<PagedResult<NotificationDto>>>
{
    private readonly IReadNotificationRepository _repo;

    public GetMyNotificationsQueryHandler(IReadNotificationRepository repo) => _repo = repo;

    public async Task<Result<PagedResult<NotificationDto>>> Handle(
        GetMyNotificationsQuery request, CancellationToken ct)
    {
        var paging = new PaginationParams { PageNumber = request.PageNumber, PageSize = request.PageSize };

        var items = await _repo.GetForUserAsync(request.UserId, request.UnreadOnly, paging, ct);
        var total = await _repo.CountUnreadForUserAsync(request.UserId, ct);

        var dtos = items.Select(n => new NotificationDto(
            n.Id, n.Type, n.Target, n.Title, n.Message,
            n.IsRead, n.ReadAt, n.ReferenceId, n.ReferenceType, n.CreatedAt)).ToList();

        return Result<PagedResult<NotificationDto>>.Success(
            PagedResult<NotificationDto>.Create(dtos, items.Count, request.PageNumber, request.PageSize));
    }
}
