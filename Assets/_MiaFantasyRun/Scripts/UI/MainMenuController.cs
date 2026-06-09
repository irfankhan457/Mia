using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MiaFantasyRun.Services;

namespace MiaFantasyRun.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string playSceneName = "Main";
        [SerializeField] private Text titleText;
        [SerializeField] private Text panelTitleText;
        [SerializeField] private Text panelBodyText;
        [SerializeField] private GameObject infoPanel;

        private BannerAdController bannerAdController;

        private void Start()
        {
            bannerAdController = FindAnyObjectByType<BannerAdController>();
            bannerAdController?.ShowMainMenuBanner();

            if (titleText != null)
            {
                titleText.text = "Mia's Fantasy Run";
            }

            ShowPanel("Welcome", "Choose Play to start running through the Fantasy City prototype.");
        }

        public void Play()
        {
            bannerAdController?.HideBanner();
            SceneManager.LoadScene(playSceneName);
        }

        public void OpenShop()
        {
            bannerAdController?.ShowShopBanner();
            ShowPanel("Shop", "Coin Shop, Gem Shop, Pet Shop, Costume Shop, Special Offers, and Daily Deals.");
        }

        public void OpenCharacters()
        {
            bannerAdController?.HideBanner();
            ShowPanel("Characters", "Mia: Free\nEmma: 1200 coins\nAurora: 2500 coins\nLuna: 1800 coins\nOlivia: 2000 coins\nSophia: 2200 coins");
        }

        public void OpenPets()
        {
            bannerAdController?.HideBanner();
            ShowPanel("Pets", "Cute Cat, White Rabbit, Baby Panda, Magic Fox, Mini Dragon, Baby Unicorn, and Baby Phoenix.");
        }

        public void OpenDailyReward()
        {
            bannerAdController?.HideBanner();
            ShowPanel("Daily Reward", "7-day reward cycle: coins, gems, costume, pet food, rare chest, and exclusive reward.");
        }

        public void OpenSettings()
        {
            bannerAdController?.HideBanner();
            ShowPanel("Settings", "Music, SFX, graphics quality, language, notifications, and privacy settings.");
        }

        public void OpenLeaderboard()
        {
            bannerAdController?.HideBanner();
            ShowPanel("Leaderboard", "Global, friends, weekly, and monthly rankings. Google Play Games integration will connect here.");
        }

        public void ClosePanel()
        {
            if (infoPanel != null)
            {
                infoPanel.SetActive(false);
            }

            bannerAdController?.ShowMainMenuBanner();
        }

        private void ShowPanel(string title, string body)
        {
            if (infoPanel != null)
            {
                infoPanel.SetActive(true);
            }

            if (panelTitleText != null)
            {
                panelTitleText.text = title;
            }

            if (panelBodyText != null)
            {
                panelBodyText.text = body;
            }
        }
    }
}
