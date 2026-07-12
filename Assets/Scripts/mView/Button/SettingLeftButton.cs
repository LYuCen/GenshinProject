using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace mView.Button
{
    public class SettingLeftButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEngine.UI.Button button;
        [SerializeField] private TextMeshProUGUI text;

        public Action OnClick;

        private void Start()
        {
            button.onClick.AddListener(() =>
            {
                OnClick?.Invoke();
            });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var color = text.color;
            color.a = 1f;
            text.color = color;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var color = text.color;
            color.a = 0.5f;
            text.color = color;
        }
    }
}