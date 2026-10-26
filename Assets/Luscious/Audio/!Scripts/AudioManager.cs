using UnityEngine;

namespace Luscious.Audio
{
    [RequireComponent(typeof(SoundMixerInterface))]
    [RequireComponent(typeof(SoundFXInterface))]
    [RequireComponent(typeof(MusicInterface))]
    public class AudioManager : Singleton<AudioManager>
    {
        SoundFXInterface _soundFXInterface;
        SoundMixerInterface _soundMixerInterface;
        MusicInterface _musicInterface;

        public SoundFXInterface SoundFX => _soundFXInterface;
        public SoundMixerInterface MixerControls => _soundMixerInterface;
        public MusicInterface Music => _musicInterface;


        protected override void OnSingletonAwake()
        {
            _soundFXInterface = GetComponent<SoundFXInterface>();
            _soundMixerInterface = GetComponent<SoundMixerInterface>();
            _musicInterface = GetComponent< MusicInterface>();
        }
    }
}
