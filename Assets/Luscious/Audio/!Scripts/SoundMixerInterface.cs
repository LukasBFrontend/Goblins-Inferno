using UnityEngine;
using UnityEngine.Audio;

namespace Luscious.Audio
{
    public class SoundMixerInterface : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [Header("Channel Multipliers")]
        [Range(0.1f, 3f)]
        [SerializeField] private float masterVolumeMultiplier = 1f;
        [Range(0.1f, 3f)]
        [SerializeField] private float soundFXVolumeMultiplier = 1f;
        [Range(0.1f, 3f)]
        [SerializeField] private float musicVolumeMultiplier = 1f;

        public AudioMixer AudioMixer => audioMixer;

        public float MasterVolume
        {
            get
            {
                audioMixer.GetFloat("masterVolume", out float volume);
                return GetOriginalPercent(volume, masterVolumeMultiplier);
            }
        }

        public float SoundFXVolume
        {
            get
            {
                audioMixer.GetFloat("soundFXVolume", out float volume);
                return GetOriginalPercent(volume, soundFXVolumeMultiplier);
            }
        }

        public float MusicVolume
        {
            get
            {
                audioMixer.GetFloat("musicVolume", out float volume);
                return GetOriginalPercent(volume, musicVolumeMultiplier);
            }
        }

        public void SetMasterVolume(float percent)
        {
            SetMixerVolume("masterVolume", percent, masterVolumeMultiplier);
        }

        public void SetSoundFXVolume(float percent)
        {
            SetMixerVolume("soundFXVolume", percent, soundFXVolumeMultiplier);
        }

        public void SetMusicVolume(float percent)
        {
            SetMixerVolume("musicVolume", percent, musicVolumeMultiplier);
        }

        public void SetLowPassFilterOn()
        {
            // TODO: Transition to Mixer Snapshots instead
            audioMixer.SetFloat("lowPassCutoffFrequency", 500f);
        }

        public void SetLowPassFilterOff()
        {
            // TODO: Transition to Mixer Snapshots instead
            audioMixer.SetFloat("lowPassCutoffFrequency", 5000f);
        }

        void Start()
        {
            SetMasterVolume(50f);
            SetSoundFXVolume(50f);
            SetMusicVolume(50f);
        }

        private void SetMixerVolume(string parameter, float percent, float multiplier)
        {
            float db = AudioUtils.PercentToDecibel(percent);
            db += MultiplierToDecibel(multiplier);
            audioMixer.SetFloat(parameter, db);
        }

        private float GetOriginalPercent(float mixerDb, float multiplier)
        {
            float multiplierDb = MultiplierToDecibel(multiplier);
            float originalDb = mixerDb - multiplierDb;
            return AudioUtils.DecibelToPercent(originalDb);
        }

        private float MultiplierToDecibel(float multiplier)
        {
            return Mathf.Log10(multiplier) * 20f;
        }
    }
}
