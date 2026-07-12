using System;
using mCommand;
using mSystem.InputSystem;
using mSystem.SceneSystem;
using mView.Button;
using QFramework;
using UnityEngine;

namespace mView.Setting
{
    public class PauseViewController : MonoBehaviour, IController
    {
        [SerializeField] private BaseButton backGame;
        [SerializeField] private BaseButton backLobby;
        [SerializeField] private BaseButton inSetting;

        public Action InSettingClick;

        private void Start()
        {
            backGame.OnClick += () => this.SendCommand(new OpenSettingCommand());
            backLobby.OnClick += () =>
            {
                this.GetSystem<ISceneSystem>().LoadSceneAsync("Lobby");
                gameObject.SetActive(false);
                GameEnter.Instance.isGame = false;
            };
            inSetting.OnClick += () =>
            {
                InSettingClick?.Invoke();
                gameObject.SetActive(false);
            };
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}