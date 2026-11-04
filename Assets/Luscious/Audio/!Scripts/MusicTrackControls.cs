using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Luscious.Audio
{
    public class MusicTrackControls : MonoBehaviour
    {
        public float Volume => _audioSource.volume;
        public float InitialVolume => _initialVolume;
        public int SceneIndex => _sceneIndex;
        private AudioSource _audioSource;
        private float _initialVolume;
        private int _sceneIndex;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Initialize(AudioMixerGroup mixerGroup, SO_SceneMusicConfig.MusicData musicData, int sceneIndex)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.loop = true;
            _audioSource.clip = musicData.audioClip;
            _initialVolume = musicData.volume;
            _audioSource.volume = musicData.volume / 100f;
            _audioSource.outputAudioMixerGroup = mixerGroup;
            _sceneIndex = sceneIndex;
        }

        public void Play(float fadeInDuration = .5f, float delay = 0f)
        {
            StartCoroutine(TransitionVolumeRoutine(0f, InitialVolume, fadeInDuration, delay));
        }

        public void Stop(float fadeOutDuration = .5f, float delay = 0f)
        {
            StartCoroutine(TransitionVolumeRoutine(Volume, 0f, fadeOutDuration, delay));
        }

        private void ResetVolume()
        {
            _audioSource.volume = _initialVolume / 100;
        }

        private IEnumerator TransitionVolumeRoutine(float startPercent, float targetPercent, float duration, float delay)
        {
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            if (startPercent <= 0f)
            {
                _audioSource.Play();
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;

                float currentPercent = Mathf.Lerp(startPercent, targetPercent, t);

                _audioSource.volume = currentPercent / 100;

                elapsed += Time.deltaTime;
                yield return null;
            }

            _audioSource.volume = targetPercent / 100;

            if (targetPercent <= 0f)
            {
                _audioSource.Stop();
                ResetVolume();
            }
        }
    }
}
