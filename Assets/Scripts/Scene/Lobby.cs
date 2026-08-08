using System;
using AssetBundleFramework.Core.Resource;
using QFramework;
using UnityEngine;

namespace Scene
{
    public class Lobby : MonoBehaviour, IController
    {
        private void Start()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Sounds/BGM/SapphireStrike.mp3", false);
            var bgm = ir.GetAsset<AudioClip>();
            AudioKit.PlayMusic(bgm, loop: true);
        }

        private void OnDestroy()
        {
            AudioKit.StopMusic();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}