using MiaFantasyRun.Data;
using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class WeatherManager : MonoBehaviour
    {
        [SerializeField] private ParticleSystem rain;
        [SerializeField] private ParticleSystem snow;
        [SerializeField] private Light sun;

        public WeatherType CurrentWeather { get; private set; }

        public void Apply(WeatherType weather)
        {
            CurrentWeather = weather;
            SetEmission(rain, weather == WeatherType.Rain);
            SetEmission(snow, weather == WeatherType.Snow);

            if (sun != null)
            {
                sun.intensity = weather is WeatherType.Night or WeatherType.Fog ? 0.55f : 1.15f;
            }
        }

        private static void SetEmission(ParticleSystem system, bool enabled)
        {
            if (system == null)
            {
                return;
            }

            if (enabled) system.Play();
            else system.Stop();
        }
    }
}
