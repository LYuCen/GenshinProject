using System.Collections.Generic;
using System.IO;
using BaseClass;
using UnityEngine;

namespace mSystem.SoundSystem
{
    public class SoundSystem : AbstractSingleton<SoundSystem>
    {
        private Dictionary<string, AudioClip> _dic;
        public AudioSource audioSource;

        private void Start()
        {
            _dic = new Dictionary<string, AudioClip>();
        }

        public void PlaySound(string str, float volume = 1)
        {
            audioSource.volume = volume;
            string resourcePath = str.Replace("Assets/Resources/", ""); // 去除前缀
            string path = Path.ChangeExtension(resourcePath, null); // 去除后缀
            
            if (!_dic.TryGetValue(path, out var clip))
            {
                clip = Resources.Load<AudioClip>(path);
                if (!clip)
                {
                    Debug.LogError("加载音效失败: " + path);
                    return;
                }
                _dic.Add(path, clip);
            }

            audioSource.PlayOneShot(_dic[path]);
        }
    }
}