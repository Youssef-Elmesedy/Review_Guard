using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.Dto;

namespace Review_Guard.Application.Feature.UserModul.Command.ChangePassword;

public sealed record ChangePasswordCommand(Guid UserId, ChangePasswordRequest Request)
    : IRequest<Result>;
