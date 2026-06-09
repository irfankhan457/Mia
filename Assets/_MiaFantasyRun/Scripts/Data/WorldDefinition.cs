using System.Collections.Generic;
using UnityEngine;

namespace MiaFantasyRun.Data
{
    public enum WeatherType
    {
        Sunny,
        Rain,
        Snow,
        Fog,
        Sunset,
        Night
    }

    [CreateAssetMenu(menuName = "Mia Fantasy Run/World")]
    public sealed class WorldDefinition : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        [TextArea] public string VisualDirection;
        public Material Skybox;
        public AudioClip Music;
        public List<GameObject> TrackSegments = new();
        public List<WeatherType> SupportedWeather = new();
    }
}
