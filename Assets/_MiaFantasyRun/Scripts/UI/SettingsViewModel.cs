using System;
using MiaFantasyRun.Services;

namespace MiaFantasyRun.UI
{
    public sealed class SettingsViewModel
    {
        private readonly SaveData saveData;

        public SettingsViewModel(SaveData saveData)
        {
            this.saveData = saveData;
        }

        public event Action<UserSettings> Changed;

        public void SetMusicVolume(float value)
        {
            saveData.Settings.MusicVolume = value;
            Changed?.Invoke(saveData.Settings);
        }

        public void SetSfxVolume(float value)
        {
            saveData.Settings.SfxVolume = value;
            Changed?.Invoke(saveData.Settings);
        }

        public void SetPersonalizedAds(bool enabled)
        {
            saveData.Settings.PersonalizedAds = enabled;
            Changed?.Invoke(saveData.Settings);
        }
    }
}
