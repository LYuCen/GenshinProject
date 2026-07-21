using BaseClass;
using mCommand;
using mEvent;
using mModel;
using QFramework;
using TMPro;
using UnityEngine;

namespace mView.TipView
{
    public class TipViewController : AbstractSingleton<TipViewController>, IController
    {
        [SerializeField] private InputTipViewController inputTip;
        [SerializeField] private TextMeshProUGUI inputTipText;

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
            inputTip.gameObject.SetActive(true);

            switch (e.Type)
            {
                case InputTipType.Name:
                    inputTipText.text = "请输入新昵称";
                    inputTip.OnApplyClick += str =>
                    {
                        this.GetModel<IUserModel>().UpdateName(str);
                        this.SendCommand(new UpdateUserInfoCommand());
                    };
                    break;
                case InputTipType.Signature:
                    inputTipText.text = "请输入新签名";
                    inputTip.OnApplyClick += str =>
                    {
                        this.GetModel<IUserModel>().UpdateSignature(str);
                        this.SendCommand(new UpdateUserInfoCommand());
                    };
                    break;
            }
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