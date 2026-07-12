using mCommand;
using mEvent;
using mModel;
using mSystem.InputSystem;
using mSystem.mSettingSystem;
using mSystem.SceneSystem;
using mView.Button;
using mView.UserView;
using QFramework;
using TMPro;
using UnityEngine;

namespace mView
{
    /// <summary>
    /// 大厅视图控制器
    /// </summary>
    public class LobbyViewController : MonoBehaviour, IController
    {
        [SerializeField] private BaseButton settingBtn; // 设置
        [SerializeField] private BaseButton quit;       // 退出
        [SerializeField] private BaseButton userBtn;    // 玩家信息
        [SerializeField] private BaseButton startBtn;   // 开始游戏
        [SerializeField] private TextMeshProUGUI userName;
        [SerializeField] private UserViewController viewController;

        private void Start()
        {
            settingBtn.OnClick += OpenSetting;
            startBtn.OnClick += StartGame;
            quit.OnClick += QuitGame;
            userBtn.OnClick += viewController.ButtonClick;
            userName.text = this.GetModel<IUserModel>().UserInfo.Name;
            
            this.RegisterEvent<UpdateUserInfoEvent>(_ =>
            {
                userName.text = this.GetModel<IUserModel>().UserInfo.Name;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OpenSetting()
        {
            this.SendCommand(new OpenSettingCommand());
        }

        private void OnDestroy()
        {
            settingBtn.OnClick -= OpenSetting;
            userBtn.OnClick -= viewController.ButtonClick;
            quit.OnClick -= QuitGame;
        }
        
        private void StartGame()
        {
            this.GetSystem<ISceneSystem>().LoadSceneAsync("Ground");
            GameEnter.Instance.isGame = true;
            this.GetSystem<ISettingSystem>().Apply();
        }

        private void QuitGame()
        {
            this.SendCommand(new TextTipCommand(Application.Quit, "是否退出游戏"));
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}