using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.UserService;

namespace Review_Guard.Application.Feature.UserModul.Command.SuspendUser;

internal sealed class SuspendUserCommandHandler : IRequestHandler<SuspendUserCommand, Result>
{
    private readonly IWriteUserService _writeUserService;

    public SuspendUserCommandHandler(IWriteUserService writeUserService)
        => _writeUserService = writeUserService;

    public async Task<Result> Handle(SuspendUserCommand request, CancellationToken cancellationToken)
        => await _writeUserService.SuspendUserAsync(request.AdminId, request.UserId, request.Request, cancellationToken);
}
