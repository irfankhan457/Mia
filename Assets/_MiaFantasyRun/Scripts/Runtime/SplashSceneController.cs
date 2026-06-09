using System.Collections;
using MiaFantasyRun.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiaFantasyRun.Runtime
{
    public sealed class SplashSceneController : MonoBehaviour
    {
        [SerializeField] private string nextSceneName = "MainMenu";
        [SerializeField] private float minimumSplashSeconds = 2f;
        [SerializeField] private Slider loadingSlider;
        [SerializeField] private Text statusText;
        [SerializeField] private Text logoText;
        [SerializeField] private Services.AnalyticsManager analyticsManager;
        [SerializeField] private Services.AdsManager adsManager;

        private IEnumerator Start()
        {
            if (logoText != null)
            {
                logoText.text = "Mia's Fantasy Run";
            }

            yield return InitializeStep("Starting magical systems", 0.15f, 0.35f);

            analyticsManager ??= FindAnyObjectByType<AnalyticsManager>();
            analyticsManager?.Initialize();
            yield return InitializeStep("Firebase ready", 0.35f, 0.65f);

            adsManager ??= FindAnyObjectByType<AdsManager>();
            adsManager?.Initialize();
            yield return InitializeStep("AdMob ready", 0.65f, 0.9f);

            var elapsed = 0f;
            while (elapsed < minimumSplashSeconds)
            {
                elapsed += Time.deltaTime;
                SetProgress(Mathf.Lerp(0.9f, 1f, elapsed / minimumSplashSeconds), "Loading adventure");
                yield return null;
            }

            SceneManager.LoadScene(nextSceneName);
        }

        private IEnumerator InitializeStep(string label, float from, float to)
        {
            var elapsed = 0f;
            const float duration = 0.45f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                SetProgress(Mathf.Lerp(from, to, elapsed / duration), label);
                yield return null;
            }
        }

        private void SetProgress(float value, string status)
        {
            if (loadingSlider != null)
            {
                loadingSlider.value = Mathf.Clamp01(value);
            }

            if (statusText != null)
            {
                statusText.text = status;
            }
        }
    }
}
