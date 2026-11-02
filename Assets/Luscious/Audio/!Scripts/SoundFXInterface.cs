using UnityEngine;
using UnityEngine.Audio;
using System.Globalization;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Animations;

namespace Luscious.Audio
{
    public class SoundFXInterface : MonoBehaviour
    {
        [SerializeField] SO_SoundFXConfig config;
        [Header("Mixer Groups")]
        [SerializeField] AudioMixerGroup mixerGroup;
        [SerializeField] AudioMixerGroup priorityMixerGroup;

        public SO_SoundFXConfig Default => config;

        /* PUBLIC METHODS */

        public SoundFXLifetimeData PlaySoundFXClip(SoundEffectData data, Vector3 position, bool sendDuckSignal = false, Transform followTarget = null)
        {
            AudioSource audioSource = CreateAudioSource(
                CreateObjectName(data.audioClip, followTarget),
                position,
                followTarget,
                sendDuckSignal ? priorityMixerGroup : mixerGroup,
                data.audioClip,
                data.volume,
                data.spatialSoundSettings,
                data.loop
            );

            StartCoroutine(PlayWithDelay(audioSource, data.playDelay));

            return new SoundFXLifetimeData(this, audioSource, audioSource.clip.length + data.playDelay);
        }

        public SoundFXLifetimeData PlayRandomSoundFXClip(MultipleSoundData data, Vector3 position, int lastIndex, out int selectedIndex, Transform followTarget = null)
        {
            if (data.clips.Count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(PlayRandomSoundFXClip));
            }

            selectedIndex = UnityEngine.Random.Range(0, data.clips.Count);
            while (selectedIndex == lastIndex)
            {
                selectedIndex = UnityEngine.Random.Range(0, data.clips.Count);
            }

            AudioSource audioSource = CreateAudioSource(
                CreateObjectName(data.clips[selectedIndex], followTarget),
                position,
                followTarget,
                mixerGroup,
                data.clips[selectedIndex],
                data.volume,
                data.spatialSoundSettings,
                false
            );

            StartCoroutine(PlayWithDelay(audioSource, data.playDelay));

            return new SoundFXLifetimeData(this, audioSource, audioSource.clip.length +  data.playDelay);
        }

        public SoundFXLifetimeData PlayRandomSoundFXClip(MultipleSoundData data, Vector3 position, Transform followTarget = null)
        {
            if (data.clips.Count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(PlayRandomSoundFXClip));
            }

            int selectedIndex = UnityEngine.Random.Range(0, data.clips.Count);

            AudioSource audioSource = CreateAudioSource(
                CreateObjectName(data.clips[selectedIndex], followTarget),
                position,
                followTarget,
                mixerGroup,
                data.clips[selectedIndex],
                data.volume,
                data.spatialSoundSettings,
                false
            );

            StartCoroutine(PlayWithDelay(audioSource, data.playDelay));

            return new SoundFXLifetimeData(this, audioSource, audioSource.clip.length + data.playDelay);
        }

        /* PRIVATE METHODS */

        private AudioSource CreateAudioSource(string name, Vector3 position, Transform followTarget, AudioMixerGroup outputMixerGroup, AudioClip clip,  float volumePercent, SpatialSoundSettings spatialSoundSettings, bool loop)
        {
            AudioSource audioSource = new GameObject(name).AddComponent<AudioSource>();

            audioSource.transform.SetParent(transform);
            audioSource.transform.position = position;
            audioSource.clip = clip;
            audioSource.volume = volumePercent / 100f;
            audioSource.loop = loop;
            audioSource.spatialBlend = spatialSoundSettings.blend;
            audioSource.minDistance = spatialSoundSettings.minDistance;
            audioSource.maxDistance = spatialSoundSettings.minDistance;
            audioSource.outputAudioMixerGroup = outputMixerGroup;

            if (followTarget)
            {
                SetPositionSource(audioSource, followTarget);
            }

            return audioSource;
        }

        private void SetPositionSource(AudioSource audioSource, Transform source)
        {
            var constraint = audioSource.AddComponent<PositionConstraint>();
            constraint.AddSource(new ConstraintSource(){ sourceTransform = source, weight = 1 });
            constraint.constraintActive = true;
        }

        private IEnumerator PlayWithDelay(AudioSource audioSource, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (audioSource == null)
            {
                yield break;
            }
            audioSource.Play();
        }

        private string CreateObjectName(AudioClip audioClip, Transform followTarget )
        {
            string result = "";

            result += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(audioClip.name.ToLower());
            result += followTarget != null ? $" : [{followTarget.name}]" : "";
            result += " (SoundFXObject)";

            return result;
        }
    }


    public class SoundFXLifetimeData
    {
        public AudioSource AudioSource => _audioSource;
        public float TimeLeft => _timeLeft;
        public bool Active => _timeLeft > 0 && _audioSource != null;
        public OnLifetimeOver OnLifeTimeOver { get => _onLifeTimeOver; set => _onLifeTimeOver = value; }

        private AudioSource _audioSource;
        private float _timeLeft;
        MonoBehaviour _monoBehavior;
        private OnLifetimeOver _onLifeTimeOver;

        public SoundFXLifetimeData(MonoBehaviour monoBehavior, AudioSource audioSource, float lifeTime)
        {
            _audioSource = audioSource;
            _timeLeft = audioSource.loop ? Mathf.Infinity : lifeTime;
            _monoBehavior = monoBehavior;

            SceneManager.sceneUnloaded += (scene) =>
            {
                if (_audioSource != null)
                {
                    _audioSource.Stop();
                }
            };
            monoBehavior.StartCoroutine(LifeTimeRoutine());
        }

        public void Destroy()
        {
            OnLifeTimeOver?.Invoke();
            AudioUtils.FadeVolume(_monoBehavior, _audioSource, .2f);
            UnityEngine.Object.Destroy(_audioSource.gameObject, .21f);
        }

        private IEnumerator LifeTimeRoutine()
        {
            while(_timeLeft > 0)
            {
                if (!Active)
                {
                    yield break;
                }

                yield return null;
                _timeLeft -= Time.deltaTime;
            }

            OnLifeTimeOver?.Invoke();
            UnityEngine.Object.Destroy(_audioSource.gameObject);
        }

        public delegate void OnLifetimeOver();
    }
}
