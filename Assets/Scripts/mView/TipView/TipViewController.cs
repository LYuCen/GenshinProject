using BaseClass;
using mEvent;
using QFramework;
using TMPro;
using UnityEngine;

namespace mView.TipView
{
    public class TipViewController : AbstractSingleton<TipViewController>, IController
    {
        [SerializeField] private InputTipViewController inputTip;

        [SerializeField] private TextTipViewController textTip;
        [SerializeField] private TextMeshProUGUI textTipText;

        private void Start()
        {
            this.RegisterEvent<InputTipEvent>(ShowTip)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<TextTipEvent>(ShowTextTip)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void ShowTip(InputTipEvent e)
        {
            inputTip.Init(e.TitleStr, e.PlaceholderStr, e.Apply, e.Abandon);
            inputTip.gameObject.SetActive(true);
        }

        private void ShowTextTip(TextTipEvent e)
        {
            textTip.gameObject.SetActive(true);
            textTipText.text = e.TipText;
            textTip.OnApplyClick = e.ApplyAction;
            textTip.OnAbandonClick = e.AbandonAction;
            textTip.OnCloseTip = e.CloseTip;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}