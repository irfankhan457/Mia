using MiaFantasyRun.Services;
using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class RewardedAdsDebugInput : MonoBehaviour
    {
        [SerializeField] private RewardedAdsRewardManager rewardedAdsRewardManager;

        private void Start()
        {
            rewardedAdsRewardManager ??= FindAnyObjectByType<RewardedAdsRewardManager>();
        }

        private void Update()
        {
            if (rewardedAdsRewardManager == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                rewardedAdsRewardManager.ShowDoubleCoinsReward();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                rewardedAdsRewardManager.ShowExtraLifeReward();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                rewardedAdsRewardManager.ShowBonusGemsReward();
            }
        }
    }
}
