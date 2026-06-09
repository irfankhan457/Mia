using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class AudioManager : MonoBehaviour, IAudioService
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource.clip == clip)
            {
                return;
            }

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void PlaySfx(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }

        public void SetMusicVolume(float volume) => musicSource.volume = Mathf.Clamp01(volume);
        public void SetSfxVolume(float volume) => sfxSource.volume = Mathf.Clamp01(volume);
    }
}
