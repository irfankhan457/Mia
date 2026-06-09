using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class AntiCheatManager : MonoBehaviour
    {
        [SerializeField] private float maxExpectedMetersPerSecond = 55f;

        private float previousDistance;
        private float previousTime;

        public bool ValidateDistance(float distanceMeters)
        {
            var deltaTime = Mathf.Max(Time.realtimeSinceStartup - previousTime, 0.001f);
            var speed = (distanceMeters - previousDistance) / deltaTime;
            previousDistance = distanceMeters;
            previousTime = Time.realtimeSinceStartup;
            return speed <= maxExpectedMetersPerSecond;
        }

        public bool ValidateCurrencyDelta(int delta)
        {
            return delta >= 0 && delta <= 10000;
        }
    }
}
