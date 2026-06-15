using MediatR;

namespace Review_Guard.Application.Feature.UserModul.Queries.GetAllUsers;

public sealed record GetAllUsersQuery(int PageNumber, int PageSize)
    : IRequest<Result<PagedResult<UserListItemDto>>>;
