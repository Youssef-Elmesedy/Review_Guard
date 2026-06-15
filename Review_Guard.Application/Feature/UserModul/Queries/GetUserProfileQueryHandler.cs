using MediatR;
using Review_Guard.Application.Feature.UserModul.UserService;

namespace Review_Guard.Application.Feature.UserModul.Queries;

internal sealed class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileResponse>>
{
    private readonly IReadUserService _readUserService;

    public GetUserProfileQueryHandler(IReadUserService readUserService)
    {
        _readUserService = readUserService;
    }

    public async Task<Result<UserProfileResponse>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userProfileResult = await _readUserService.GetProfileAsync(request.UserId, cancellationToken);

        return userProfileResult;
    }
}
