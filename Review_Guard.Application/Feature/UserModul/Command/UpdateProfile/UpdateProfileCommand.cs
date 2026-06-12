using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.Dto;

namespace Review_Guard.Application.Feature.UserModul.Command.UpdateProfile;

public sealed record UpdateProfileCommand(Guid UserId, UpdateProfileRequest Request)
    : IRequest<Result>;
