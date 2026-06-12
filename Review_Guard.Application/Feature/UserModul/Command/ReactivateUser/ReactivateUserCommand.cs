using MediatR;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.UserModul.Command.ReactivateUser;

public sealed record ReactivateUserCommand(Guid AdminId, Guid UserId) : IRequest<Result>;
