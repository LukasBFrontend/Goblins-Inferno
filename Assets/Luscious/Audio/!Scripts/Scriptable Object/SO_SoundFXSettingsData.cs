using System.Collections.Generic;
using UnityEngine;

namespace Luscious.Audio
{
    [CreateAssetMenu(menuName = "Luscious/Audio/SoundFX Config")]
    public class SO_SoundFXConfig : ScriptableObject
    {
        [Header("SoundFX Data")]
        [Header("UI")]
        public SO_SoundEffectData buttonPressSound;
        [Header("Game State")]
        public SO_SoundEffectData gameOverSound;
        public SO_SoundEffectData lvlStartSound;
        public SO_SoundEffectData levelUpSound;
        [Header("Scene Items")]
        public SO_SoundEffectData pickupSound;
        public SO_SoundEffectData chestOpenSound;
        [Header("Movement")]
        public SO_SoundEffectData jumpLandSound;
        public FootstepsSoundData footsteps;

    }

    [System.Serializable]
    public struct SO_SoundEffectData
    {
        [Range(0, 100)] public float volume;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public struct FootstepsSoundData
    {
        [Range(0, 100)] public float volume;
        public List<AudioClip> clips;
    }
}
