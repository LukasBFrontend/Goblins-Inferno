using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicInterface : MonoBehaviour
{
    [SerializeField] AudioMixerGroup audioMixerGroup;
    [SerializeField] private SO_SceneMusicConfig musicSettings;
    Dictionary<int, BackgroundMusic> _sceneMusicLookUp = new();
    readonly Dictionary<int, SO_SceneMusicConfig.TrackSettings> _sceneMusicSettingsLookUp = new();

    private void Start()
    {
        musicSettings.tracks.ForEach((musicSetting) =>
        {
            BackgroundMusic music = new GameObject(musicSetting.name).AddComponent<BackgroundMusic>();
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
            !_sceneMusicLookUp.TryGetValue(scene.buildIndex, out BackgroundMusic targetMusic)
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
            !_sceneMusicLookUp.TryGetValue(scene.buildIndex, out BackgroundMusic targetMusic)
            || !_sceneMusicSettingsLookUp.TryGetValue(scene.buildIndex, out SO_SceneMusicConfig.TrackSettings musicSettings)
        )
        {
            return;
        }

        targetMusic.TransitionVolume(targetMusic.Volume, 0f, musicSettings.fadeOutDuration, 0f);
    }

}
