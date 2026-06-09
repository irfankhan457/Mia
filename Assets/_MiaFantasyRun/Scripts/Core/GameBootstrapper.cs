using MiaFantasyRun.Services;
using UnityEngine;

namespace MiaFantasyRun.Core
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        private static GameBootstrapper instance;

        public static ServiceRegistry Services { get; private set; }

        [SerializeField] private SaveManager saveManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private AnalyticsManager analyticsManager;
        [SerializeField] private AdsManager adsManager;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            saveManager ??= GetComponent<SaveManager>();
            audioManager ??= GetComponent<AudioManager>();
            analyticsManager ??= GetComponent<AnalyticsManager>();
            adsManager ??= GetComponent<AdsManager>();

            Services = new ServiceRegistry();
            Services.Register<ISaveService>(saveManager);
            Services.Register<IAudioService>(audioManager);
            Services.Register<IAnalyticsService>(analyticsManager);
            Services.Register<IAdsService>(adsManager);

            saveManager.Initialize();
            analyticsManager.Initialize();
            adsManager.Initialize();
        }
    }
}
