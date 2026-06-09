using UnityEngine;

namespace MiaFantasyRun.Data
{
    public enum PowerUpType
    {
        CoinMagnet,
        CoinMultiplier,
        Shield,
        Jetpack,
        SpeedBoost,
        GemMultiplier,
        Invincibility,
        TimeSlowdown,
        PetBoost,
        MegaCoinRush
    }

    [CreateAssetMenu(menuName = "Mia Fantasy Run/Power Up")]
    public sealed class PowerUpDefinition : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public PowerUpType Type;
        public Sprite Icon;
        public GameObject VfxPrefab;
        public float DurationSeconds = 8f;
        public float Multiplier = 2f;
    }
}
