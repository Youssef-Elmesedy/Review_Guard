using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.Dto;
using Review_Guard.Application.Feature.UserModul.UserService;

namespace Review_Guard.Application.Feature.UserModul.Command.Query.GetUserActivities;

internal sealed class GetUserActivitiesQueryHandler
    : IRequestHandler<GetUserActivitiesQuery, Result<PagedResult<UserActivityDto>>>
{
    private readonly IReadUserService _readUserService;

    public GetUserActivitiesQueryHandler(IReadUserService readUserService)
        => _readUserService = readUserService;

    public async Task<Result<PagedResult<UserActivityDto>>> Handle(
        GetUserActivitiesQuery request, CancellationToken cancellationToken)
    {
        var paging = new PaginationParams
        {
            PageNumber = request.PageNumber,
            PageSize   = request.PageSize
        };

        return await _readUserService.GetUserActivitiesAsync(request.UserId, paging, cancellationToken);
    }
}
