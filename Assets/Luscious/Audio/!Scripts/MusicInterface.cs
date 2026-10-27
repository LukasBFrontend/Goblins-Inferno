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
        private bool _playMusic;
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
            _playMusic = autoPlayOnSceneLoad;
        }


        private void Start()
        {
            musicSettings.tracks.ForEach((musicSetting) =>
            {
                MusicTrackControls music = new GameObject(musicSetting.name).AddComponent<MusicTrackControls>();
                music.transform.SetParent(transform);
                music.Initialize(mixerGroup, musicSetting.data, musicSetting.sceneIndex);

                _sceneMusicLookUp.Add(musicSetting.sceneIndex, music);
                _sceneMusicSettingsLookUp.Add(musicSetting.sceneIndex, musicSetting);
            });

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            if (_audioManager.Mixer.AudioMixer.GetFloat(LOWPASS_CUTOFF_FREQ, out float cutoffFreq))
            {
                _startCutoffFreq = cutoffFreq;
            }

            PlaySceneMusic(SceneManager.GetActiveScene());
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PlaySceneMusic(scene);
        }

        private void OnSceneUnloaded(Scene scene)
        {
            // TODO: Fade out the volume for the entire music AudioMixerGroup for the purpose of avoiding sound artifacts when scene is unloaded.
            if (
                !_sceneMusicLookUp.TryGetValue(scene.buildIndex, out MusicTrackControls targetMusic)
                || !_sceneMusicSettingsLookUp.TryGetValue(scene.buildIndex, out SO_SceneMusicConfig.TrackSettings musicSettings)
            )
            {
                return;
            }

            targetMusic.TransitionVolume(targetMusic.Volume, 0f, musicSettings.fadeOutDuration, 0f);
        }

        private void PlaySceneMusic(Scene scene)
        {
            if (
                !_playMusic
                || !_sceneMusicLookUp.TryGetValue(scene.buildIndex, out MusicTrackControls targetMusic)
                || !_sceneMusicSettingsLookUp.TryGetValue(scene.buildIndex, out SO_SceneMusicConfig.TrackSettings musicSettings)
            )
            {
                return;
            }

            targetMusic.Play(musicSettings.timeAfterSceneLoadPlay);
            targetMusic.TransitionVolume(0f, targetMusic.InitialVolume, musicSettings.fadeInDuration, musicSettings.timeAfterSceneLoadPlay);
        }

    }
}
