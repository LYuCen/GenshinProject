using System;
using AssetBundleFramework.Core.Resource;
using QFramework;
using TMPro;
using UnityEngine;

namespace mView.Button
{
    public class SettingTextButton : MonoBehaviour
    {
        [SerializeField] private SettingLeftButton left;
        [SerializeField] private SettingLeftButton right;
        [SerializeField] private TextMeshProUGUI text;

        public Action<int> OnClick;
        public AudioClip clip;
        private void Start()
        {
            left.OnClick += () =>
            {
                OnClick?.Invoke(-1);
                AudioKit.PlayVoice(clip);
            };
            
            right.OnClick += () =>
            {
                OnClick?.Invoke(1);
                AudioKit.PlayVoice(clip);
            };
        }

        public void SetText(string str)
        {
            text.text = str;
        }
    }
}