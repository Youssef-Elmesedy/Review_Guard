using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.Dto;

namespace Review_Guard.Application.Feature.UserModul.Command.Query.GetAllUsers;

public sealed record GetAllUsersQuery(int PageNumber, int PageSize)
    : IRequest<Result<PagedResult<UserListItemDto>>>;
