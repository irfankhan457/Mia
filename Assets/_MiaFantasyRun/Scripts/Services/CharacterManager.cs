using System.Linq;
using System.Collections.Generic;
using MiaFantasyRun.Data;
using MiaFantasyRun.Core;
using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class CharacterManager : MonoBehaviour
    {
        [SerializeField] private GameCatalog catalog;
        [SerializeField] private List<CharacterData> characterData = new();

        private IAnalyticsService analyticsService;

        public IReadOnlyList<CharacterData> CharacterData => characterData;

        public CharacterData GetCharacterData(string characterName)
        {
            return characterData.FirstOrDefault(character => character.characterName == characterName);
        }

        public bool IsUnlocked(CharacterData character, SaveData saveData)
        {
            return character != null && saveData.OwnedCharacters.Contains(character.characterName.ToLowerInvariant());
        }

        public bool TryUnlock(CharacterData character, SaveData saveData)
        {
            if (character == null)
            {
                return false;
            }

            var id = character.characterName.ToLowerInvariant();
            if (saveData.OwnedCharacters.Contains(id))
            {
                return true;
            }

            if (saveData.Coins < character.unlockCost)
            {
                return false;
            }

            saveData.Coins -= character.unlockCost;
            saveData.OwnedCharacters.Add(id);
            TrackCharacterUnlocked(character.characterName, character.unlockCost.ToString());
            return true;
        }

        public CharacterDefinition SelectedCharacter(SaveData saveData)
        {
            return catalog.Characters.FirstOrDefault(character => character.Id == saveData.SelectedCharacterId)
                   ?? catalog.Characters.FirstOrDefault();
        }

        public bool TryUnlock(CharacterDefinition character, SaveData saveData)
        {
            if (saveData.OwnedCharacters.Contains(character.Id))
            {
                return true;
            }

            if (saveData.Coins < character.CoinPrice || saveData.Gems < character.GemPrice)
            {
                return false;
            }

            saveData.Coins -= character.CoinPrice;
            saveData.Gems -= character.GemPrice;
            saveData.OwnedCharacters.Add(character.Id);
            TrackCharacterUnlocked(character.Id, $"{character.CoinPrice}:{character.GemPrice}");
            return true;
        }

        public bool Select(string characterId, SaveData saveData)
        {
            if (!saveData.OwnedCharacters.Contains(characterId))
            {
                return false;
            }

            saveData.SelectedCharacterId = characterId;
            return true;
        }

        private void TrackCharacterUnlocked(string characterId, string cost)
        {
            ResolveAnalytics();
            analyticsService?.TrackEvent(AnalyticsEvents.CharacterUnlocked, ("character", characterId), ("cost", cost));
        }

        private void ResolveAnalytics()
        {
            if (analyticsService != null || GameBootstrapper.Services == null)
            {
                return;
            }

            analyticsService = GameBootstrapper.Services.Resolve<IAnalyticsService>();
        }
    }
}
