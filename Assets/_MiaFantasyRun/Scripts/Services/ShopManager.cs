using MiaFantasyRun.Data;
using MiaFantasyRun.Core;
using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class ShopManager : MonoBehaviour
    {
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private PetManager petManager;
        [SerializeField] private SaveManager saveManager;

        private IAnalyticsService analyticsService;

        public bool BuyCharacter(CharacterDefinition character)
        {
            var save = saveManager.Load();
            var alreadyOwned = save.OwnedCharacters.Contains(character.Id);
            var purchased = characterManager.TryUnlock(character, save);
            if (purchased)
            {
                saveManager.Save(save);
                if (!alreadyOwned)
                {
                    TrackPurchaseCompleted("character", character.Id, character.CoinPrice, character.GemPrice);
                }
            }

            return purchased;
        }

        public bool BuyPet(PetDefinition pet)
        {
            var save = saveManager.Load();
            var alreadyOwned = save.OwnedPets.Contains(pet.Id);
            var purchased = petManager.TryUnlock(pet, save);
            if (purchased)
            {
                saveManager.Save(save);
                if (!alreadyOwned)
                {
                    TrackPurchaseCompleted("pet", pet.Id, 0, pet.UnlockCostGems);
                }
            }

            return purchased;
        }

        private void TrackPurchaseCompleted(string itemType, string itemId, int coinCost, int gemCost)
        {
            ResolveAnalytics();
            analyticsService?.TrackEvent(
                AnalyticsEvents.PurchaseCompleted,
                ("item_type", itemType),
                ("item_id", itemId),
                ("coin_cost", coinCost.ToString()),
                ("gem_cost", gemCost.ToString()));
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
