using System;
using DG.Tweening;
using mView.Button;
using TMPro;
using UnityEngine;

namespace mView.TipView
{
    public class InputTipViewController : MonoBehaviour
    {
        [SerializeField] private RectTransform tip;
        [SerializeField] private BaseButton apply;
        [SerializeField] private BaseButton abandon;
        [SerializeField] private TMP_InputField inputField;
        
        public Action<string> OnApplyClick;
        public Action OnAbandonClick;
        public Action OnCloseTip;

        public void Start()
        {
            apply.OnClick += () =>
            {
                OnApplyClick?.Invoke(inputField.text);
                OnClose();
            };

            abandon.OnClick += () =>
            {
                OnAbandonClick?.Invoke();
                OnClose();
            };
        }

        public void OnEnable()
        {
            tip.localScale = new Vector3(1, 0, 1);
            tip.DOScale(Vector3.one, 0.2f); 
        }

        private void OnClose()
        {
            tip.DOScale(new Vector3(1, 0, 1), 0.2f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                inputField.text = "";
                ClearListener();
                OnCloseTip?.Invoke();
            });
        }

        private void ClearListener()
        {
            OnApplyClick = null;
            OnAbandonClick = null;
        }
    }
}