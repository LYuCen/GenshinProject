using mWeapon.ComponentData;
using UnityEngine;

namespace mWeapon.Component
{
    public class AudioComponent
    {
        public AudioClip GetAudioClip => _data.getAudioClip;
        public AudioClip FireAudioClip => _data.fireAudioClip;
        public AudioClip ReloadAudioClip => _data.reloadAudioClip;
        public AudioClip BoltAudioClip => _data.boltAudioClip;

        private AudioComponentData _data;
        
        public AudioComponent(AudioComponentData data)
        {
            _data = data;
        }
    }
}