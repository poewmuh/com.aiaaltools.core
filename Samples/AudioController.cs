using AiaalTools.Data.Saver;
using UnityEngine;
using UnityEngine.Audio;

namespace AiaalTools.Samples.Audio
{
    public class AudioController : MonoSingleton<AudioController>
    {
        private const string MASTER_VOLUME = "Master";
        private const string MUSIC_VOLUME = "Music";
        private const string EFFECTS_VOLUME = "Effects";
        private const float MINIMAL_VOLUME = -50;
        private const float MAXIMAL_VOLUME = 0f;

        [SerializeField] private AudioMixer _mainMixer;

        private void Start()
        {
            var appSettings = ProfileData.GetLocalData<ApplicationSettingsData>();
            OnMasterVolumeChange(appSettings.MasterVolume);
        }

        public static void PlayEvent(AudioSource audioSource)
        {
            Debug.Log($"[AudioController] PlayEvent: {audioSource.clip}");
            audioSource.Play();
        }

        public static void StopEvent(AudioSource audioSource)
        {
            Debug.Log($"[AudioController] StopEvent: {audioSource.clip}");
            audioSource.Stop();
        }

        public void OnMasterVolumeChange(float newValue)
        {
            _mainMixer.SetFloat(MASTER_VOLUME, GetDBFromValue(newValue));
        }

        public void OnMusicVolumeChange(float newValue)
        {
            _mainMixer.SetFloat(MUSIC_VOLUME, GetDBFromValue(newValue));
        }

        public void OnEffectsVolumeChange(float newValue)
        {
            _mainMixer.SetFloat(EFFECTS_VOLUME, GetDBFromValue(newValue));
        }

        private float GetDBFromValue(float volume)
        {
            if (volume.ApproxZero())
            {
                return -80f;
            }
            return Mathf.Lerp(MINIMAL_VOLUME, MAXIMAL_VOLUME, volume);
        }
    }
}