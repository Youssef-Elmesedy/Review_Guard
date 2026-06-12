using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.UserService;

namespace Review_Guard.Application.Feature.UserModul.Command.ReactivateUser;

internal sealed class ReactivateUserCommandHandler : IRequestHandler<ReactivateUserCommand, Result>
{
    private readonly IWriteUserService _writeUserService;

    public ReactivateUserCommandHandler(IWriteUserService writeUserService)
        => _writeUserService = writeUserService;

    public async Task<Result> Handle(ReactivateUserCommand request, CancellationToken cancellationToken)
        => await _writeUserService.ReactivateUserAsync(request.AdminId, request.UserId, cancellationToken);
}
