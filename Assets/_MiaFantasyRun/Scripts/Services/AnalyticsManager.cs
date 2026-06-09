using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class AnalyticsManager : MonoBehaviour, IAnalyticsService
    {
        public void Initialize()
        {
#if FIREBASE_ANALYTICS
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
#endif
        }

        public void TrackEvent(string eventName, params (string key, string value)[] parameters)
        {
#if FIREBASE_ANALYTICS
            var firebaseParameters = new Firebase.Analytics.Parameter[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                firebaseParameters[i] = new Firebase.Analytics.Parameter(parameters[i].key, parameters[i].value);
            }
            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, firebaseParameters);
#else
            Debug.Log($"Analytics event: {eventName}");
#endif
        }
    }
}
