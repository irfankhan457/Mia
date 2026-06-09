using UnityEngine;

namespace MiaFantasyRun.Services
{
    public interface IAudioService
    {
        void PlayMusic(AudioClip clip);
        void PlaySfx(AudioClip clip);
        void SetMusicVolume(float volume);
        void SetSfxVolume(float volume);
    }
}
