using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.UserService;

namespace Review_Guard.Application.Feature.UserModul.Command.UpdateProfile;

internal sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly IWriteUserService _writeUserService;

    public UpdateProfileCommandHandler(IWriteUserService writeUserService)
        => _writeUserService = writeUserService;

    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        => await _writeUserService.UpdateProfileAsync(request.UserId, request.Request, cancellationToken);
}
