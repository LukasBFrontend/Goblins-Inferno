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
            public string name;
            public int sceneIndex;
            public MusicData data;
            [Space]
            public float timeAfterSceneLoadPlay;
            public float fadeInDuration;
            public float fadeOutDuration;

        }

        [System.Serializable]
        public struct MusicData
        {
            public AudioClip audioClip;
            [Range(0, 100)] public float volume;
        }
    }
}
