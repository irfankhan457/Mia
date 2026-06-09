using MiaFantasyRun.Core;
using MiaFantasyRun.Runtime;
using MiaFantasyRun.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiaFantasyRun.Gameplay
{
    public sealed class RunnerGameManager : MonoBehaviour
    {
        [SerializeField] private float baseSpeed = 8f;
        [SerializeField] private float difficultyRampPerMinute = 1.4f;
        [SerializeField] private PlayerController player;

        private readonly GameState state = new();
        private IAnalyticsService analytics;
        private ISaveService saveService;
        private IAdsService adsService;
        private InterstitialAdFrequencyController interstitialFrequencyController;
        private float speed;
        private bool savedGameOver;
        private bool usedRewardedExtraLife;
        private int savedRunCoins;
        private int savedRunGems;

        public GameState State => state;
        public float Speed => speed;
        public bool IsGameOver => state.RunState == RunState.GameOver;

        private void Start()
        {
            analytics = GameBootstrapper.Services.Resolve<IAnalyticsService>();
            saveService = GameBootstrapper.Services.Resolve<ISaveService>();
            adsService = GameBootstrapper.Services.Resolve<IAdsService>();
            adsService.HideBanner();
            interstitialFrequencyController ??= FindAnyObjectByType<InterstitialAdFrequencyController>();
            player ??= FindFirstObjectByType<PlayerController>();
            StartRun(GameMode.Endless);
        }

        private void Update()
        {
            if (state.RunState == RunState.GameOver && Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }

            if (state.RunState != RunState.Running)
            {
                return;
            }

            speed = baseSpeed + state.DistanceMeters / 220f * difficultyRampPerMinute;
            state.AddDistance(speed * Time.deltaTime);
        }

        public void StartRun(GameMode mode)
        {
            speed = baseSpeed;
            savedGameOver = false;
            usedRewardedExtraLife = false;
            savedRunCoins = 0;
            savedRunGems = 0;
            state.Begin(mode);
            player ??= FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.ResetRunner();
            }

            analytics.TrackEvent(AnalyticsEvents.GameStarted, ("mode", mode.ToString()));
        }

        public void CollectCoin(int amount)
        {
            state.AddCoins(amount);
            analytics.TrackEvent(AnalyticsEvents.CoinCollected, ("amount", amount.ToString()));
        }

        public void CollectGem(int amount)
        {
            state.AddGems(amount);
            analytics.TrackEvent(AnalyticsEvents.GemCollected, ("amount", amount.ToString()));
        }

        public void DoubleCurrentRunCoins()
        {
            if (state.Coins <= 0)
            {
                return;
            }

            state.AddCoins(state.Coins);
            analytics.TrackEvent("reward_double_coins", ("coins", state.Coins.ToString()));
            if (IsGameOver)
            {
                SaveRunProgress();
            }
        }

        public bool ReviveFromRewardedAd()
        {
            if (!savedGameOver || usedRewardedExtraLife)
            {
                return false;
            }

            usedRewardedExtraLife = true;
            savedGameOver = false;
            state.Resume();
            player ??= FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.ResetRunner();
            }

            analytics.TrackEvent("reward_extra_life");
            return true;
        }

        public void GameOver()
        {
            if (savedGameOver)
            {
                return;
            }

            savedGameOver = true;
            state.Finish();
            SaveRunProgress();
            interstitialFrequencyController ??= FindAnyObjectByType<InterstitialAdFrequencyController>();
            interstitialFrequencyController?.RegisterRunCompleted();
            analytics.TrackEvent(AnalyticsEvents.GameCompleted, ("score", state.Score.ToString()), ("distance", state.DistanceMeters.ToString("F0")));
        }

        private void SaveRunProgress()
        {
            var save = saveService.Load();
            var coinDelta = Mathf.Max(0, state.Coins - savedRunCoins);
            var gemDelta = Mathf.Max(0, state.Gems - savedRunGems);
            save.Coins += coinDelta;
            save.Gems += gemDelta;
            save.HighScore = Mathf.Max(save.HighScore, state.Score);
            save.LongestDistanceMeters = Mathf.Max(save.LongestDistanceMeters, state.DistanceMeters);
            saveService.Save(save);
            savedRunCoins = state.Coins;
            savedRunGems = state.Gems;
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        }
    }
}
