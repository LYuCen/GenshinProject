using mCommand;
using mEvent;
using mModel;
using mSystem.mSettingSystem;
using mView.Button;
using mView.Room;
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
        [SerializeField] private BaseButton createGameBtn;   // 创建游戏
        [SerializeField] private BaseButton joinGameBtn;    // 加入游戏
        [SerializeField] private TextMeshProUGUI userName;
        [SerializeField] private UserViewController viewController;

        [SerializeField] private LobbyRoomView lobbyRoomView;
        private void Start()
        {
            settingBtn.OnClick += OpenSetting;
            createGameBtn.OnClick = lobbyRoomView.ShowCreateRoomView;
            joinGameBtn.OnClick = lobbyRoomView.ShowJoinRoomView;
            quit.OnClick += QuitGame;
            userBtn.OnClick += viewController.ButtonClick;
            userName.text = this.GetModel<IUserModel>().UserInfo.Name;
            
            this.RegisterEvent<UpdateUserInfoEvent>(_ =>
            {
                userName.text = this.GetModel<IUserModel>().UserInfo.Name;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            // 初始化设置信息
            this.GetSystem<ISettingSystem>().Apply();
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
            
        }

        private void QuitGame()
        {
            this.SendCommand(new TextTipCommand("是否退出游戏",Application.Quit, null, null));
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}