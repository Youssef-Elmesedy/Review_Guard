namespace Review_Guard.Application.Abstractions.Services.RewardService;

public interface IRewardService
{
    Task GrantRewardsAsync(Guid userId, CancellationToken cancellationToken = default);
}
