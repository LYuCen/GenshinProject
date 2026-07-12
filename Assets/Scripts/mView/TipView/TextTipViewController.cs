using System;
using DG.Tweening;
using mView.Button;
using UnityEngine;

namespace mView.TipView
{
    public class TextTipViewController : MonoBehaviour
    {
        [SerializeField] private RectTransform tip;
        [SerializeField] private BaseButton apply;
        [SerializeField] private BaseButton abandon;

        public Action OnApplyClick;
        public Action OnAbandonClick;
        public Action OnCloseTip;

        public void Start()
        {
            apply.OnClick += () =>
            {
                OnApplyClick?.Invoke();
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

        public void OnClose()
        {
            tip.DOScale(new Vector3(1, 0, 1), 0.2f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                OnCloseTip?.Invoke();
            });
        }
    }
}