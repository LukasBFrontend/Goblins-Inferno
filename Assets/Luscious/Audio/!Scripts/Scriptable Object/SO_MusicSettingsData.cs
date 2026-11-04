using System.Collections.Generic;
using UnityEngine;

namespace Luscious.Audio
{
    [CreateAssetMenu(menuName = "Luscious/Audio/Scene Music Config")]
    public class SO_SceneMusicConfig : ScriptableObject
    {
        public List<TrackSettings> tracks;

        [System.Serializable]
        public struct TrackSettings
        {
            public string id;
            public int sceneIndex;
            public MusicData data;
            [Space]
            public AutoPlaySettings autoPlaySettings;

        }

        [System.Serializable]
        public struct MusicData
        {
            public AudioClip audioClip;
            [Range(0, 100)] public float volume;
        }

        [System.Serializable]
        public struct AutoPlaySettings
        {
            public float timeAfterSceneLoadPlay;
            public float fadeInDuration;
            public float fadeOutDuration;
        }
    }
}
