using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace mView.Button
{
    public class SettingMainButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject backGround;
        [SerializeField] private GameObject enterImg;
        [SerializeField] private UnityEngine.UI.Button button;
        [SerializeField] private TextMeshProUGUI text;

        public Action OnClick;
        [SerializeField] private bool isChose;

        private void Start()
        {
            button.onClick.AddListener(() =>
            {
                OnClick?.Invoke();
                EnterChose();
            });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isChose) return;
            enterImg.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isChose) return;
            enterImg.SetActive(false);
        }

        public void EnterChose()
        {
            var color = text.color;
            color.a = 1f;
            text.color = color;
            backGround.SetActive(true);
            enterImg.SetActive(false);
            isChose = true;
        }

        public void ExitChose()
        {
            var color = text.color;
            color.a = 0.5f;
            text.color = color;
            backGround.SetActive(false);
            isChose = false;
        }
    }
}