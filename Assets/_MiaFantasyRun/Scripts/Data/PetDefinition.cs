using UnityEngine;

namespace MiaFantasyRun.Data
{
    public enum PetAbilityType
    {
        CoinBoost,
        GemBoost,
        ExtraShield,
        DistanceBonus,
        ScoreMultiplier,
        MagnetExtension
    }

    [CreateAssetMenu(menuName = "Mia Fantasy Run/Pet")]
    public sealed class PetDefinition : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public Sprite Portrait;
        public GameObject Prefab;
        public PetAbilityType AbilityType;
        public float AbilityValue = 1.1f;
        public int UnlockCostGems;
    }
}
