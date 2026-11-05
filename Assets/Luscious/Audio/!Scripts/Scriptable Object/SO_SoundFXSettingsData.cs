using System.Collections.Generic;
using UnityEngine;

namespace Luscious.Audio
{
    [CreateAssetMenu(menuName = "Luscious/Audio/SoundFX Config")]
    public class SO_SoundFXConfig : ScriptableObject
    {
        [Header("SoundFX Data")]
        [Header("UI")]
        public SoundEffectData buttonPressSound;
        [Header("Game State")]
        public SoundEffectData gameOverSound;
        public SoundEffectData lvlStartSound;
        public SoundEffectData levelUpSound;
        [Header("Scene Items")]
        public MultipleSoundData pickupSound;
        public SoundEffectData chestOpenSound;
        [Header("Movement")]
        public SoundEffectData jumpLandSound;
        public MultipleSoundData footsteps;
    }

    [System.Serializable]
    public struct SoundEffectData
    {
        public AudioClip audioClip;
        [Range(0, 100)] public float volume;
        public bool loop;
        public float playDelay;
        [Space]
        public SpatialSoundSettings spatialSoundSettings;
    }

    [System.Serializable]
    public struct MultipleSoundData
    {
        public List<AudioClip> clips;
        [Range(0, 100)] public float volume;
        public float playDelay;
        [Space]
        public SpatialSoundSettings spatialSoundSettings;
    }

    [System.Serializable]
    public struct SpatialSoundSettings
    {
        [Range(0f, 1f)] public float blend;
        public float minDistance;
        public float maxDistance;
    }
}
