using System;
using MiaFantasyRun.Runtime;

namespace MiaFantasyRun.UI
{
    public sealed class HomeViewModel
    {
        public event Action<GameMode> PlayRequested;
        public event Action<ScreenId> ScreenRequested;

        public void PlayEndless() => PlayRequested?.Invoke(GameMode.Endless);
        public void PlayDailyChallenge() => PlayRequested?.Invoke(GameMode.DailyChallenge);
        public void OpenShop() => ScreenRequested?.Invoke(ScreenId.Shop);
        public void OpenCharacters() => ScreenRequested?.Invoke(ScreenId.CharacterSelection);
        public void OpenPets() => ScreenRequested?.Invoke(ScreenId.PetSelection);
    }
}
