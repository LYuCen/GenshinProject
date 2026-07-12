using System;
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
        private void Start()
        {
            left.OnClick += () =>
            {
                OnClick?.Invoke(-1);
            };
            
            right.OnClick += () =>
            {
                OnClick?.Invoke(1);
            };
        }

        public void SetText(string str)
        {
            text.text = str;
        }
    }
}