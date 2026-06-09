using System;
using System.Collections.Generic;

namespace MiaFantasyRun.Services
{
    [Serializable]
    public sealed class SaveData
    {
        public int Coins;
        public int Gems;
        public int HighScore;
        public float LongestDistanceMeters;
        public string SelectedCharacterId = "mia";
        public string SelectedPetId = "";
        public int DailyRewardDay;
        public string LastDailyRewardDate = "";
        public List<string> OwnedCharacters = new() { "mia" };
        public List<string> OwnedPets = new();
        public List<string> OwnedOutfits = new();
        public List<string> CompletedAchievements = new();
        public List<string> ClaimedMissions = new();
        public UserSettings Settings = new();
    }

    [Serializable]
    public sealed class UserSettings
    {
        public float MusicVolume = 0.8f;
        public float SfxVolume = 0.9f;
        public int GraphicsQuality = 2;
        public string Language = "en";
        public bool Notifications = true;
        public bool PersonalizedAds = false;
    }
}
