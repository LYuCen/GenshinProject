using System;
using System.Collections.Generic;
using mSystem.mSettingSystem.ItemData;
using mView.Button;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace mView.Setting.SettingItemView
{
    public class SettingDropdownView : MonoBehaviour, ISettingItemView, IPointerEnterHandler, IPointerExitHandler
    {
        public TextMeshProUGUI label;
        public int index;
        [SerializeField] private GameObject enterImg;
        [SerializeField] private SettingTextButton btn;
        [SerializeField] private List<string> options;

        public Action OnUpdate;
        public Action<string> OnHoverIntroduce;

        private string _introduce;
        
        private void Start()
        {
            btn.OnClick += OnClick;
        }
        
        public void Build(SettingItemData data)
        {
            label.text = data.name;
            _introduce = data.introduce;
            
            options = data.options;
            index = data.value;
            btn.SetText(options[index]);
        }

        public void SetIndex(int idx)
        {
            index = idx;
            btn.SetText(options[index]);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            enterImg.SetActive(true);
            OnHoverIntroduce?.Invoke(_introduce); 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            enterImg.SetActive(false);
        }

        private void OnDisable()
        {
            OnHoverIntroduce?.Invoke(""); 
        }

        private void OnClick(int i)
        {
            index = (index + i + options.Count) % options.Count;
            btn.SetText(options[index]);
            OnUpdate?.Invoke();
        }
    }
}