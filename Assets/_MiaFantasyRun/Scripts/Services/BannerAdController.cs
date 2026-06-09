using MiaFantasyRun.Core;
using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class BannerAdController : MonoBehaviour
    {
        private IAdsService adsService;

        private void Start()
        {
            ResolveAds();
        }

        public void ShowMainMenuBanner()
        {
            Show(BannerAdPlacement.MainMenu);
        }

        public void ShowShopBanner()
        {
            Show(BannerAdPlacement.Shop);
        }

        public void HideBanner()
        {
            ResolveAds();
            adsService?.HideBanner();
        }

        private void Show(BannerAdPlacement placement)
        {
            ResolveAds();
            adsService?.ShowBanner(placement.ToString());
        }

        private void ResolveAds()
        {
            if (adsService != null || GameBootstrapper.Services == null)
            {
                return;
            }

            adsService = GameBootstrapper.Services.Resolve<IAdsService>();
        }
    }
}
