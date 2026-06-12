using MediatR;
using Review_Guard.Application.Abstractions.Repositories.NotificationRepository;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.NotificationModule.DTOs;

namespace Review_Guard.Application.Feature.NotificationModule.Queries.GetAdminNotifications;

internal sealed class GetAdminNotificationsQueryHandler
    : IRequestHandler<GetAdminNotificationsQuery, Result<PagedResult<NotificationDto>>>
{
    private readonly IReadNotificationRepository _repo;

    public GetAdminNotificationsQueryHandler(IReadNotificationRepository repo) => _repo = repo;

    public async Task<Result<PagedResult<NotificationDto>>> Handle(
        GetAdminNotificationsQuery request, CancellationToken ct)
    {
        var paging = new PaginationParams { PageNumber = request.PageNumber, PageSize = request.PageSize };

        var items = await _repo.GetForAdminAsync(request.AdminId, request.UnreadOnly, paging, ct);

        var dtos = items.Select(n => new NotificationDto(
            n.Id, n.Type, n.Target, n.Title, n.Message,
            n.IsRead, n.ReadAt, n.ReferenceId, n.ReferenceType, n.CreatedAt)).ToList();

        return Result<PagedResult<NotificationDto>>.Success(
            PagedResult<NotificationDto>.Create(dtos, items.Count, request.PageNumber, request.PageSize));
    }
}
