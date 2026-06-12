using Review_Guard.Application.Abstractions.Repositories.GenericRepository;
using Review_Guard.Application.Feature.UserModul.Dto;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Abstractions.Repositories.UserRepository;

public interface IReadUserRepository : IGenericReadRepository<User>
{
    Task<User?> GetByIdWithRewardsAsync(
    Guid id,
    CancellationToken cancellationToken = default);

    Task<UserReviewStats> GetUserReviewStatsAsync(
    Guid userId,
    CancellationToken cancellationToken = default);
}
