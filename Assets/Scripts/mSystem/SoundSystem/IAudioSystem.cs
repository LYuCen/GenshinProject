using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using QFramework;
using UnityEngine;

namespace mSystem.SoundSystem
{
    public interface IAudioSystem : ISystem
    {
        void PlayMusic(string str, float val = 1f);
        void PlayMusic(AudioClip clip, float val = 1f);
        void PlaySound(string str, float val = 1f);
        void PlaySound(AudioClip clip, float val = 1f);
    }

    public class AudioSystem : AbstractSystem, IAudioSystem
    {
        private Dictionary<string, AudioClip> _dic;
        protected override void OnInit()
        {
            _dic = new Dictionary<string, AudioClip>();
        }

        public void PlayMusic(string str, float val = 1)
        {
            if (!_dic.TryGetValue(str, out var clip))
            {
                var ir = ResourceManager.instance.Load(str, false);
                AudioClip c = ir.GetAsset<AudioClip>();
                _dic.Add(str, c);
            }
            AudioKit.PlayMusic(_dic[str]);
        }

        public void PlayMusic(AudioClip clip, float val = 1)
        {
            AudioKit.PlayMusic(clip);
        }

        public void PlaySound(string str, float val = 1)
        {
            if (!_dic.TryGetValue(str, out var clip))
            {
                var ir = ResourceManager.instance.Load(str, false);
                AudioClip c = ir.GetAsset<AudioClip>();
                _dic.Add(str, c);
            }
            AudioKit.PlaySound(_dic[str]);
        }

        public void PlaySound(AudioClip clip, float val = 1)
        {
            AudioKit.PlaySound(clip);
        }
    }
}