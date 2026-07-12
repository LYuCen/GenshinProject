using AssetBundleFramework.Core.Resource;
using BaseClass;
using Unity.VisualScripting;
using UnityEngine;

namespace mSystem.SoundSystem
{
    public class AudioSystem : AbstractSingleton<AudioSystem>
    {
        public AudioSource audioSource;
        private IResource _ir;

        protected override void Awake()
        {
            base.Awake();
            audioSource = transform.AddComponent<AudioSource>();
        }

        public void PlaySound(AudioClip audioClip, float volume = 1)
        {
            if (audioClip)
            {
                audioSource.PlayOneShot(audioClip, volume);
            } 
        }

        public void SetVolumeValue(int val)
        {
            audioSource.volume = val / 100f;
        }
    }
}