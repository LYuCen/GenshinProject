using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace mView.Button
{
    public class BaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject backGround;
        [SerializeField] private GameObject enterImg;
        [SerializeField] private UnityEngine.UI.Button button;

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
            enterImg.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            enterImg.SetActive(false);
        }

        public void OnEnable()
        {
            button.enabled = true;
        }

        public void OnDisable()
        {
            button.enabled = false;
            enterImg.SetActive(false);
        }
    }
}