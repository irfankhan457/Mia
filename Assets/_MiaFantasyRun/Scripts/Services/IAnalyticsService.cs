namespace MiaFantasyRun.Services
{
    public interface IAnalyticsService
    {
        void Initialize();
        void TrackEvent(string eventName, params (string key, string value)[] parameters);
    }
}
