using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Luscious.Audio
{
    public class MusicInterface : MonoBehaviour
    {
        [SerializeField] AudioMixerGroup audioMixerGroup;
        [SerializeField] private SO_SceneMusicConfig musicSettings;
        [Space()]
        [SerializeField] bool playAutoOnSceneLoad = true;
        Dictionary<int, MusicTrackControls> _sceneMusicLookUp = new();
        readonly Dictionary<int, SO_SceneMusicConfig.TrackSettings> _sceneMusicSettingsLookUp = new();
        bool _playMusic;

        private void Awake()
        {
            _playMusic = playAutoOnSceneLoad;
        }

        private void Start()
        {
            musicSettings.tracks.ForEach((musicSetting) =>
            {
                MusicTrackControls music = new GameObject(musicSetting.name).AddComponent<MusicTrackControls>();
                music.transform.SetParent(transform);
                music.Initialize(audioMixerGroup, musicSetting.data, musicSetting.sceneIndex);

                _sceneMusicLookUp.Add(musicSetting.sceneIndex, music);
                _sceneMusicSettingsLookUp.Add(musicSetting.sceneIndex, musicSetting);
            });

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            PlaySceneMusic(SceneManager.GetActiveScene());
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

    }
}
