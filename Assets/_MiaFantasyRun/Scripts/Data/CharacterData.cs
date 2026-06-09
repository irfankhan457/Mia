using UnityEngine;

namespace MiaFantasyRun.Data
{
    [CreateAssetMenu(menuName = "Mia Fantasy Run/Character Data")]
    public sealed class CharacterData : ScriptableObject
    {
        public string characterName;
        public int unlockCost;
        public GameObject prefab;
    }
}
