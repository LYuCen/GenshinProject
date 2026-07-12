using System;
using mCommand;
using mEvent;
using mModel;
using mView.Button;
using mView.TipView;
using QFramework;
using TMPro;
using UnityEngine;

namespace mView.UserView
{
    public class UserViewController : MonoBehaviour, IController
    {
        [SerializeField] private BaseButton setName;
        [SerializeField] private BaseButton setSignature;

        [SerializeField] private TextMeshProUGUI userName;
        [SerializeField] private TextMeshProUGUI userSignature;

        private void Start()
        {
            setName.OnClick += () =>
            {
                this.SendCommand(new InputTipCommand(InputTipType.Name));
            };
            setSignature.OnClick += () =>
            {
                this.SendCommand(new InputTipCommand(InputTipType.Signature));
            };

            this.RegisterEvent<UpdateUserInfoEvent>(_ =>
            {
                UpdateInfo();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public void ButtonClick()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        private void OnEnable()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            var model = this.GetModel<IUserModel>();
            userName.text = model.UserInfo.Name;
            userSignature.text = model.UserInfo.Signature;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}