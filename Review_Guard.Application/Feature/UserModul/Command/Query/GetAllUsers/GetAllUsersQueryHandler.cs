using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.Dto;
using Review_Guard.Application.Feature.UserModul.UserService;

namespace Review_Guard.Application.Feature.UserModul.Command.Query.GetAllUsers;

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
            PageSize   = request.PageSize
        };

        return await _readUserService.GetAllUsersAsync(paging, cancellationToken);
    }
}
