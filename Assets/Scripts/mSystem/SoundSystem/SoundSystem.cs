using System.Collections.Generic;
using System.IO;
using AssetBundleFramework.Core.Resource;
using BaseClass;
using UnityEngine;

namespace mSystem.SoundSystem
{
    public class SoundSystem : AbstractSingleton<SoundSystem>
    {
        private Dictionary<string, AudioClip> _dic;
        public AudioSource audioSource;
        private IResource _ir;

        private void Start()
        {
            _dic = new Dictionary<string, AudioClip>();
            
        }

        public void PlaySound(string str, float volume = 1)
        {
            audioSource.volume = volume;
            
            if (!_dic.TryGetValue(str, out var clip))
            {
                _ir = ResourceManager.instance.Load(str, false);
                clip = _ir.GetAsset<AudioClip>();
                if (!clip)
                {
                    Debug.LogError("加载音效失败: " + str);
                    return;
                }
                _dic.Add(str, clip);
            }

            audioSource.PlayOneShot(_dic[str]);
        }

        public void PlaySound(AudioClip audioClip, float volume = 1)
        {
            if (audioClip)
            {
                audioSource.PlayOneShot(audioClip, volume);
            } 
        }
    }
}