using System;
using DG.Tweening;
using mView.Button;
using TMPro;
using UnityEngine;

namespace mView.TipView
{
    public class InputTipViewController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI placeholder;
        [SerializeField] private RectTransform tip;
        [SerializeField] private BaseButton apply;
        [SerializeField] private BaseButton abandon;
        [SerializeField] private TMP_InputField inputField;
        
        private Action<string> _onApplyClick;
        private Action _onAbandonClick;
        
        private void OnEnable()
        {
            tip.localScale = new Vector3(1, 0, 1);
            tip.DOScale(Vector3.one, 0.2f);

            apply.OnClick += HandleApply;
            abandon.OnClick += HandleAbandon;
        }

        public void Init(string titleStr, string placeholderStr, Action<string> onApplyClick, Action onAbandonClick)
        {
            title.text = titleStr;
            placeholder.text = placeholderStr;
            _onApplyClick = onApplyClick;
            _onAbandonClick = onAbandonClick;
        }

        private void OnClose()
        {
            tip.DOScale(new Vector3(1, 0, 1), 0.2f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                inputField.text = "";
            });
        }

        private void HandleApply()
        {
            _onApplyClick?.Invoke(inputField.text);
            OnClose();
        }

        private void HandleAbandon()
        {
            _onAbandonClick?.Invoke();
            OnClose();
        }

        private void OnDisable()
        {
            apply.OnClick -= HandleApply;
            abandon.OnClick -= HandleAbandon;

            _onApplyClick = null;
            _onAbandonClick = null;
        }
    }
}