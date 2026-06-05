using Review_Guard.Application.Abstractions.Repositories.UserRepository;
using Review_Guard.Application.Abstractions.Services.RewardService;
using Review_Guard.Domain.Servcies;

namespace Review_Guard.Infrastructure.Implementation.Servcices.RewardService;

internal class RewardService : IRewardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReadUserRepository _userRepository;

    public RewardService(IReadUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task GrantRewardsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdWithRewardsAsync(userId);

        if (user is null)
            return;

        var eligibleRewards = RewardPolicy.GetEligibleRewards(user.TrustScoreValue);

        foreach (var reward in eligibleRewards)
            user.AddReward(reward);

        await _unitOfWork.SaveChangesAsync();
    }
}
