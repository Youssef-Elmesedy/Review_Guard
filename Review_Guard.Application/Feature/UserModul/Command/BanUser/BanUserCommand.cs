using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.Dto;

namespace Review_Guard.Application.Feature.UserModul.Command.BanUser;

public sealed record BanUserCommand(Guid AdminId, Guid UserId, BanUserRequest Request)
    : IRequest<Result>;
