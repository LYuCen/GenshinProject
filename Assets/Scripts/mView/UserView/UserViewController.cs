using mCommand;
using mEvent;
using mModel;
using mView.Button;
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
                this.SendCommand(new InputTipCommand("提示", "请输入新昵称", UpdateName, null));
            };
            setSignature.OnClick += () =>
            {
                this.SendCommand(new InputTipCommand("提示", "请输入新签名", UpdateSignature, null));
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

        private void UpdateName(string str)
        {
            this.GetModel<IUserModel>().UpdateName(str);
            this.SendCommand(new UpdateUserInfoCommand());
        }

        private void UpdateSignature(string str)
        {
            this.GetModel<IUserModel>().UpdateSignature(str);
            this.SendCommand(new UpdateUserInfoCommand());
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}