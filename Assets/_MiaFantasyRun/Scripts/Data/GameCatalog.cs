using System.Collections.Generic;
using UnityEngine;

namespace MiaFantasyRun.Data
{
    [CreateAssetMenu(menuName = "Mia Fantasy Run/Game Catalog")]
    public sealed class GameCatalog : ScriptableObject
    {
        public List<CharacterDefinition> Characters = new();
        public List<PetDefinition> Pets = new();
        public List<WorldDefinition> Worlds = new();
        public List<PowerUpDefinition> PowerUps = new();
        public List<MissionDefinition> Missions = new();
    }
}
