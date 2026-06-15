using MediatR;

namespace Review_Guard.Application.Feature.UserModul.Queries;

public sealed record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserProfileResponse>>;
