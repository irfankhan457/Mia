using System;

namespace MiaFantasyRun.Services
{
    public interface IAdsService
    {
        void Initialize();
        bool IsRewardedReady { get; }
        void ShowRewarded(string placement, Action<bool> completed);
        void ShowInterstitial(string placement);
        void ShowBanner(string placement);
        void HideBanner();
    }
}
