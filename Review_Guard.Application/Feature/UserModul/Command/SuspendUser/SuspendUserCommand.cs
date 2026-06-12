using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.Dto;

namespace Review_Guard.Application.Feature.UserModul.Command.SuspendUser;

public sealed record SuspendUserCommand(Guid AdminId, Guid UserId, SuspendUserRequest Request)
    : IRequest<Result>;
