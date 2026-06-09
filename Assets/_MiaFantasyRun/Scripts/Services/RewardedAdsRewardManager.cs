using System;
using MiaFantasyRun.Core;
using MiaFantasyRun.Gameplay;
using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class RewardedAdsRewardManager : MonoBehaviour
    {
        [SerializeField] private int bonusGemsAmount = 5;
        [SerializeField] private RunnerGameManager runnerGameManager;

        private IAdsService adsService;
        private ISaveService saveService;
        private IAnalyticsService analyticsService;

        private void Awake()
        {
            runnerGameManager ??= FindAnyObjectByType<RunnerGameManager>();
        }

        private void Start()
        {
            ResolveServices();
        }

        private void ResolveServices()
        {
            if (GameBootstrapper.Services == null)
            {
                return;
            }

            adsService ??= GameBootstrapper.Services.Resolve<IAdsService>();
            saveService ??= GameBootstrapper.Services.Resolve<ISaveService>();
            analyticsService ??= GameBootstrapper.Services.Resolve<IAnalyticsService>();
            runnerGameManager ??= FindAnyObjectByType<RunnerGameManager>();
        }

        public bool CanShowRewarded()
        {
            ResolveServices();
            return adsService != null && adsService.IsRewardedReady;
        }

        public void ShowDoubleCoinsReward(Action<bool> completed = null)
        {
            ShowRewarded(RewardedAdRewardType.DoubleCoins, completed);
        }

        public void ShowExtraLifeReward(Action<bool> completed = null)
        {
            ShowRewarded(RewardedAdRewardType.ExtraLife, completed);
        }

        public void ShowBonusGemsReward(Action<bool> completed = null)
        {
            ShowRewarded(RewardedAdRewardType.BonusGems, completed);
        }

        public void ShowRewarded(RewardedAdRewardType rewardType, Action<bool> completed = null)
        {
            if (!CanShowRewarded())
            {
                completed?.Invoke(false);
                return;
            }

            var placement = rewardType.ToString();
            analyticsService?.TrackEvent("rewarded_ad_requested", ("reward", placement));
            adsService.ShowRewarded(placement, watched =>
            {
                if (watched)
                {
                    ApplyReward(rewardType);
                    analyticsService?.TrackEvent(AnalyticsEvents.RewardedAdWatched, ("reward", placement));
                }
                else
                {
                    analyticsService?.TrackEvent("rewarded_ad_skipped", ("reward", placement));
                }

                completed?.Invoke(watched);
            });
        }

        private void ApplyReward(RewardedAdRewardType rewardType)
        {
            runnerGameManager ??= FindAnyObjectByType<RunnerGameManager>();

            switch (rewardType)
            {
                case RewardedAdRewardType.DoubleCoins:
                    runnerGameManager?.DoubleCurrentRunCoins();
                    break;
                case RewardedAdRewardType.ExtraLife:
                    runnerGameManager?.ReviveFromRewardedAd();
                    break;
                case RewardedAdRewardType.BonusGems:
                    ApplyBonusGems();
                    break;
            }
        }

        private void ApplyBonusGems()
        {
            if (runnerGameManager != null && !runnerGameManager.IsGameOver)
            {
                runnerGameManager.CollectGem(bonusGemsAmount);
                return;
            }

            if (saveService == null)
            {
                return;
            }

            var save = saveService.Load();
            save.Gems += bonusGemsAmount;
            saveService.Save(save);
        }
    }
}
