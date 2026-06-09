using UnityEngine;

namespace MiaFantasyRun.Data
{
    [CreateAssetMenu(menuName = "Mia Fantasy Run/Character")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public Sprite Portrait;
        public GameObject Prefab;
        public AudioClip VoiceClip;
        public RuntimeAnimatorController AnimatorController;
        public bool CanHover = true;
        public int CoinPrice;
        public int GemPrice;
    }
}
