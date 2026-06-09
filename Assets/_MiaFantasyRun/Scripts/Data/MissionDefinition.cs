using UnityEngine;

namespace MiaFantasyRun.Data
{
    public enum MissionMetric
    {
        Coins,
        Gems,
        DistanceMeters,
        CharactersUnlocked,
        DailyChallengeCompleted,
        JetpackUses,
        Slides,
        Jumps
    }

    [CreateAssetMenu(menuName = "Mia Fantasy Run/Mission")]
    public sealed class MissionDefinition : ScriptableObject
    {
        public string Id;
        public string Title;
        public MissionMetric Metric;
        public int TargetValue;
        public int RewardCoins;
        public int RewardGems;
    }
}
