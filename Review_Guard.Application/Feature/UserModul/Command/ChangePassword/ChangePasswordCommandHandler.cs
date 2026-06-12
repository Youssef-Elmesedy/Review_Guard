using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.UserService;

namespace Review_Guard.Application.Feature.UserModul.Command.ChangePassword;

internal sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IWriteUserService _writeUserService;

    public ChangePasswordCommandHandler(IWriteUserService writeUserService)
        => _writeUserService = writeUserService;

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        => await _writeUserService.ChangePasswordAsync(request.UserId, request.Request, cancellationToken);
}
