using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace mView.Button
{
    public class SetKeyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEngine.UI.Button button;
        [SerializeField] private TextMeshProUGUI text;
        [Header("虚拟键")] public string actionName;

        public Action<String, SetKeyButton> OnClick;
        [SerializeField] private bool isChose;

        private void Start()
        {
            button.onClick.AddListener(() =>
            {
                OnClick?.Invoke(actionName, this);
                EnterChose();
            });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isChose) return;
            var color = text.color;
            color.a = 1f;
            text.color = color;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isChose) return;
            var color = text.color;
            color.a = 0.5f;
            text.color = color;
        }

        public void EnterChose()
        {
            var color = text.color;
            color.a = 1f;
            text.color = color;
            isChose = true;
        }

        public void ExitChose()
        {
            var color = text.color;
            color.a = 0.5f;
            text.color = color;
            isChose = false;
        }

        public void SetText(string str)
        {
            text.text = str;
        }
    }
}