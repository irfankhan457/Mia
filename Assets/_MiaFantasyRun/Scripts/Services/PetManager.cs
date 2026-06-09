using System.Linq;
using MiaFantasyRun.Data;
using MiaFantasyRun.Core;
using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class PetManager : MonoBehaviour
    {
        [SerializeField] private GameCatalog catalog;

        private IAnalyticsService analyticsService;

        public PetDefinition SelectedPet(SaveData saveData)
        {
            if (string.IsNullOrEmpty(saveData.SelectedPetId))
            {
                return null;
            }

            return catalog.Pets.FirstOrDefault(pet => pet.Id == saveData.SelectedPetId);
        }

        public bool TryUnlock(PetDefinition pet, SaveData saveData)
        {
            if (saveData.OwnedPets.Contains(pet.Id))
            {
                return true;
            }

            if (saveData.Gems < pet.UnlockCostGems)
            {
                return false;
            }

            saveData.Gems -= pet.UnlockCostGems;
            saveData.OwnedPets.Add(pet.Id);
            ResolveAnalytics();
            analyticsService?.TrackEvent(AnalyticsEvents.PetUnlocked, ("pet", pet.Id), ("cost_gems", pet.UnlockCostGems.ToString()));
            return true;
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
