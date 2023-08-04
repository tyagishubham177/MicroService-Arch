using ShubT.Services.RewardAPI.Message;

namespace ShubT.Services.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardsMessage rewardsMessage);
    }
}
