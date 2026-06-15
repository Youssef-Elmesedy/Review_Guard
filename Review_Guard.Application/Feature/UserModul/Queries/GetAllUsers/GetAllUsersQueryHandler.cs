using MediatR;
using Review_Guard.Application.Feature.UserModul.UserService;

namespace Review_Guard.Application.Feature.UserModul.Queries.GetAllUsers;

internal sealed class GetAllUsersQueryHandler
    : IRequestHandler<GetAllUsersQuery, Result<PagedResult<UserListItemDto>>>
{
    private readonly IReadUserService _readUserService;

    public GetAllUsersQueryHandler(IReadUserService readUserService)
        => _readUserService = readUserService;

    public async Task<Result<PagedResult<UserListItemDto>>> Handle(
        GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var paging = new PaginationParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return await _readUserService.GetAllUsersAsync(paging, cancellationToken);
    }
}
