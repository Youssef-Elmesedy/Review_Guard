using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.UserService;

namespace Review_Guard.Application.Feature.UserModul.Command.BanUser;

internal sealed class BanUserCommandHandler : IRequestHandler<BanUserCommand, Result>
{
    private readonly IWriteUserService _writeUserService;

    public BanUserCommandHandler(IWriteUserService writeUserService)
        => _writeUserService = writeUserService;

    public async Task<Result> Handle(BanUserCommand request, CancellationToken cancellationToken)
        => await _writeUserService.BanUserAsync(request.AdminId, request.UserId, request.Request, cancellationToken);
}
