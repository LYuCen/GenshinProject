using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace mView.Button
{
    public class SettingSecondButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject backGround;
        [SerializeField] private GameObject lowImg;
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
            backGround.SetActive(true);
            lowImg.SetActive(false);
            isChose = true;
        }

        public void ExitChose()
        {
            var color = text.color;
            color.a = 0.5f;
            text.color = color;
            backGround.SetActive(false);
            lowImg.SetActive(true);
            isChose = false;
        }
    }
}