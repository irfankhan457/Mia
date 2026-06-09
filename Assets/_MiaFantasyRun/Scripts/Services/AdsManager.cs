using System;
using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class AdsManager : MonoBehaviour, IAdsService
    {
        [SerializeField] private bool testMode = true;
        [SerializeField] private string androidRewardedUnitId = "ca-app-pub-3940256099942544/5224354917";
        [SerializeField] private string androidInterstitialUnitId = "ca-app-pub-3940256099942544/1033173712";
        [SerializeField] private string androidBannerUnitId = "ca-app-pub-3940256099942544/6300978111";

        public bool IsRewardedReady { get; private set; }
        public bool IsBannerVisible { get; private set; }

        public void Initialize()
        {
            IsRewardedReady = true;
            Debug.Log($"Ads initialized. Test mode: {testMode}. Rewarded: {androidRewardedUnitId}. Interstitial: {androidInterstitialUnitId}. Banner: {androidBannerUnitId}");
        }

        public void ShowRewarded(string placement, Action<bool> completed)
        {
            Debug.Log($"Rewarded ad requested: {placement}");
            completed?.Invoke(true);
        }

        public void ShowInterstitial(string placement)
        {
            Debug.Log($"Interstitial ad requested: {placement}");
        }

        public void ShowBanner(string placement)
        {
            IsBannerVisible = true;
            Debug.Log($"Banner ad shown: {placement}. Unit: {androidBannerUnitId}");
        }

        public void HideBanner()
        {
            if (!IsBannerVisible)
            {
                return;
            }

            IsBannerVisible = false;
            Debug.Log("Banner ad hidden");
        }
    }
}
