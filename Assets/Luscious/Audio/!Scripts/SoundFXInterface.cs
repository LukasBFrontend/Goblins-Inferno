using UnityEngine;
using UnityEngine.Audio;
using System.Globalization;

namespace Luscious.Audio
{
    public class SoundFXInterface : MonoBehaviour
    {
        [SerializeField] SO_SoundFXConfig config;
        [Header("Mixer Groups")]
        [SerializeField] AudioMixerGroup mixerGroup;
        [SerializeField] AudioMixerGroup priorityMixerGroup;

        private readonly AudioSource _repeatSource = null;
        private float _timer = 0f;
        private float _repeatDelay = 0f;
        private int _iterationsLeft = 0;

        public SO_SoundFXConfig Default => config;

        /* PUBLIC METHODS */

        /// <summary>
        /// Play a sound effect once.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="spawnTransform">The transform which will be used to determine where the SoundFXObject is spawned.</param>
        /// <param name="volume">The relative volume at which to play the AudioClip(s). Expected to be a value between 0f and 100f.</param>
        public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, bool sendDuckSignal = false)
        {
            AudioSource audioSource = new GameObject(CreateObjectName(audioClip)).AddComponent<AudioSource>();

            audioSource.transform.SetParent(transform);
            audioSource.transform.position = spawnTransform.position;
            audioSource.clip = audioClip;
            audioSource.volume = Mathf.Log10(volume / 100 + 1);
            audioSource.Play();
            audioSource.outputAudioMixerGroup = sendDuckSignal ? priorityMixerGroup : mixerGroup;

            float clipLength = audioSource.clip.length + 1f;

            Destroy(audioSource.gameObject, clipLength);
        }

        /// <summary>
        /// Play a sound effect once.
        /// </summary>
        /// <param name="soundFXData"></param>
        /// <param name="spawnTransform">The transform which will be used to determine where the SoundFXObject is spawned.</param>
        /// <param name="volume">The relative volume at which to play the AudioClip(s). Expected to be a value between 0f and 100f.</param>
        public void PlaySoundFXClip(SO_SoundEffectData soundFXData, Transform spawnTransform, bool sendDuckSignal = false)
        {
            AudioSource audioSource = new GameObject(CreateObjectName(soundFXData.audioClip)).AddComponent<AudioSource>();

            audioSource.transform.SetParent(transform);
            audioSource.transform.position = spawnTransform.position;
            audioSource.clip = soundFXData.audioClip;
            audioSource.volume = Mathf.Log10(soundFXData.volume / 100 + 1);
            audioSource.Play();
            audioSource.outputAudioMixerGroup = sendDuckSignal ? priorityMixerGroup : mixerGroup;

            float clipLength = audioSource.clip.length + 1f;

            Destroy(audioSource.gameObject, clipLength);
        }

        /// <summary>
        /// Play a random sound effect from an array of AudioClips.
        /// </summary>
        /// <param name="audioClips"></param>
        /// <param name="spawnTransform">The transform which will be used to determine where the SoundFXObject is spawned.</param>
        /// <param name="volume">The relative volume at which to play the AudioClip(s). Expected to be a value between 0f and 100f.</param>
        public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume)
        {
            int rand = Random.Range(0, audioClips.Length);

            AudioSource audioSource = new GameObject(CreateObjectName(audioClips[0])).AddComponent<AudioSource>();

            audioSource.transform.SetParent(transform);
            audioSource.transform.position = spawnTransform.position;
            audioSource.clip = audioClips[rand];
            audioSource.volume = Mathf.Log10(volume / 100 + 1);
            audioSource.Play();

            float clipLength = audioSource.clip.length;

            Destroy(audioSource.gameObject, clipLength);
        }

        /// <summary>
        /// Play a sound effect a given amount of times. SoundFXManager can currently only keep track of one repeating sound effect.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="spawnTransform">The transform which will be used to determine where the SoundFXObject is spawned.</param>
        /// <param name="volume">The relative volume at which to play the AudioClip(s). Expected to be a value between 0f and 100f.</param>
        /// <param name="iterations"></param>
        /// <param name="delay">The interval between invokes.</param>
        public void PlaySoundFXtimes(AudioClip audioClip, Transform spawnTransform, float volume, int iterations, float delay)
        {
            _iterationsLeft = iterations;
            _repeatDelay = delay;

            AudioSource audioSource = new GameObject(CreateObjectName(audioClip)).AddComponent<AudioSource>();

            audioSource.transform.SetParent(transform);
            audioSource.transform.position = spawnTransform.position;
            _repeatSource.clip = audioClip;
            _repeatSource.volume = Mathf.Log10(volume / 100 + 1);

            float clipLength = _repeatSource.clip.length;

            Destroy(_repeatSource.gameObject, iterations * (clipLength + delay));
        }

        /* MONOBEHAVIOR LIFECYCLE METHODS */

        private void Update()
        {
            if (_timer <= 0 && _iterationsLeft > 0)
            {
                _repeatSource.Play();
                _timer = _repeatSource.clip.length + _repeatDelay;
                _iterationsLeft--;
            }

            _timer -= Time.deltaTime;
        }

        /* PRIVATE METHODS */

        private string CreateObjectName(AudioClip audioClip)
        {
            return $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(audioClip.name.ToLower())} (SoundFXObject)";
        }

        private void OnGameOver()
        {
            PlaySoundFXClip(config.gameOverSound.audioClip, transform, config.gameOverSound.volume);
        }
    }
}
