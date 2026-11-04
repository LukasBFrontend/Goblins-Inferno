using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Luscious.Audio
{
    [RequireComponent(typeof(AudioManager))]
    public class MusicInterface : MonoBehaviour
    {
        const string LOWPASS_CUTOFF_FREQ = "lowpassCutoffFreq";

        [SerializeField] private SO_SceneMusicConfig musicSettings;
        [SerializeField] AudioMixerGroup mixerGroup;
        [Range(0f, 10000f)]
        [SerializeField] float lowPassCutoffFrequency = 3000f;
        [Space()]
        [SerializeField] bool autoPlayOnSceneLoad = true;

        private readonly Dictionary<int, MusicTrackControls> _sceneMusicLookUp = new();
        private readonly Dictionary<int, SO_SceneMusicConfig.TrackSettings> _sceneMusicSettingsLookUp = new();
        private AudioManager _audioManager;
        private float _startCutoffFreq;

        public void SetLowPassOn()
        {
             _audioManager.Mixer.AudioMixer.SetFloat(LOWPASS_CUTOFF_FREQ, lowPassCutoffFrequency);
        }

        public void SetLowPassOff()
        {
             _audioManager.Mixer.AudioMixer.SetFloat(LOWPASS_CUTOFF_FREQ, _startCutoffFreq);
        }

        private void Awake()
        {
            _audioManager = GetComponent<AudioManager>();
        }


        private void Start()
        {
            musicSettings.tracks.ForEach((musicSetting) =>
            {
                MusicTrackControls music = new GameObject(musicSetting.id).AddComponent<MusicTrackControls>();
                music.transform.SetParent(transform);
                music.Initialize(mixerGroup, musicSetting.data, musicSetting.sceneIndex);

                _sceneMusicLookUp.Add(musicSetting.sceneIndex, music);
                _sceneMusicSettingsLookUp.Add(musicSetting.sceneIndex, musicSetting);
            });

            if (_audioManager.Mixer.AudioMixer.GetFloat(LOWPASS_CUTOFF_FREQ, out float cutoffFreq))
            {
                _startCutoffFreq = cutoffFreq;
            }

            if (autoPlayOnSceneLoad)
            {
                SceneManager.sceneLoaded += AutoOnSceneLoaded;
                SceneManager.sceneUnloaded += AutoOnSceneUnloaded;
                PlaySceneMusic(SceneManager.GetActiveScene());
            }
        }

        private void AutoOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var autoPlaySettings = _sceneMusicSettingsLookUp[scene.buildIndex].autoPlaySettings;

            float fadeInDuration = autoPlaySettings.fadeInDuration;
            float delay = autoPlaySettings.timeAfterSceneLoadPlay;
            PlaySceneMusic(scene, fadeInDuration, delay);
        }

        private void AutoOnSceneUnloaded(Scene scene)
        {
            var autoPlaySettings = _sceneMusicSettingsLookUp[scene.buildIndex].autoPlaySettings;

            float fadeOutDuration = autoPlaySettings.fadeInDuration;
            StopSceneMusic(scene, fadeOutDuration);
        }

        public void PlaySceneMusic(Scene scene, float fadeInDuration = .5f, float delay = 0f)
        {
            if (
                !_sceneMusicLookUp.TryGetValue(scene.buildIndex, out MusicTrackControls targetMusic)
                || !_sceneMusicSettingsLookUp.TryGetValue(scene.buildIndex, out SO_SceneMusicConfig.TrackSettings musicSettings)
            )
            {
                return;
            }

            targetMusic.Play(fadeInDuration, delay);
        }

        public void StopSceneMusic(Scene scene, float fadeOutDuration = .5f, float delay = 0f)
        {
            if (
                !_sceneMusicLookUp.TryGetValue(scene.buildIndex, out MusicTrackControls targetMusic)
                || !_sceneMusicSettingsLookUp.TryGetValue(scene.buildIndex, out SO_SceneMusicConfig.TrackSettings musicSettings)
            )
            {
                return;
            }

            targetMusic.Stop(fadeOutDuration, delay);
        }
    }
}
