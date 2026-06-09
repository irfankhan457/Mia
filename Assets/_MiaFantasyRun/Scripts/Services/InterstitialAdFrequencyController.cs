using MiaFantasyRun.Core;
using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class InterstitialAdFrequencyController : MonoBehaviour
    {
        private const string CompletedRunsKey = "mia_completed_runs";

        [SerializeField] private int showEveryRuns = 3;

        private IAdsService adsService;
        private IAnalyticsService analyticsService;

        public int CompletedRuns => PlayerPrefs.GetInt(CompletedRunsKey, 0);

        private void Start()
        {
            ResolveServices();
        }

        public void RegisterRunCompleted()
        {
            ResolveServices();

            var completedRuns = CompletedRuns + 1;
            PlayerPrefs.SetInt(CompletedRunsKey, completedRuns);
            PlayerPrefs.Save();
            analyticsService?.TrackEvent("run_count_incremented", ("completed_runs", completedRuns.ToString()));

            if (showEveryRuns > 0 && completedRuns % showEveryRuns == 0)
            {
                adsService?.ShowInterstitial($"Every{showEveryRuns}Runs");
                analyticsService?.TrackEvent("interstitial_every_runs_shown", ("completed_runs", completedRuns.ToString()));
            }
        }

        private void ResolveServices()
        {
            if (GameBootstrapper.Services == null)
            {
                return;
            }

            adsService ??= GameBootstrapper.Services.Resolve<IAdsService>();
            analyticsService ??= GameBootstrapper.Services.Resolve<IAnalyticsService>();
        }
    }
}
