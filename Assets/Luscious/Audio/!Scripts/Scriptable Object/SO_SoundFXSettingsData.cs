using UnityEngine;

namespace Luscious.Audio
{
    [CreateAssetMenu(menuName = "Luscious/Audio/SoundFX Config")]
    public class SO_SoundFXConfig : ScriptableObject
    {
        [Header("SoundFX Data")]
        public SO_SoundEffectData buttonPressSound;
        public SO_SoundEffectData gameOverSound;
        public SO_SoundEffectData lvlStartSound;
        public SO_SoundEffectData pickupSound;
        public SO_SoundEffectData chestOpenSound;
        public SO_SoundEffectData jumpLandSound;

        [System.Serializable]
        public struct SO_SoundEffectData
        {
            public AudioClip audioClip;
            [Range(0, 100)] public float volume;
        }
    }
}
