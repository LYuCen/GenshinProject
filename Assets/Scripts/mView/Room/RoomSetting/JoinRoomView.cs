using System;
using Connect;
using Connect.DTO;
using mCommand;
using mSystem.SoundSystem;
using mView.Button;
using QFramework;
using UnityEngine;

namespace mView.Room.RoomSetting
{
    public class JoinRoomView : MonoBehaviour, IController
    {
        [SerializeField] private BaseButton searchBtn;
        [SerializeField] private BaseButton closeBtn;

        [SerializeField] private Transform serverContent;

        public Action<RoomInfoDto> OnJoinRoom;
        private void Start()
        {
            searchBtn.OnClick += DirectConnect;
            closeBtn.OnClick = () =>
            {
                gameObject.SetActive(false);
                this.GetSystem<IAudioSystem>().PlaySound(
                    "Assets/AssetBundle/Sounds/UI/ESM_Battle_Game_UI_Interface_Menu_Clip_Button_Smooth_Warm_Soft_1_Hybrid_One_Shot 1.wav");

            };
        }

        private void DirectConnect()
        {
            this.SendCommand(new InputTipCommand("IP直连", "请输入房间IP地址", ConnectServer, null));
            this.GetSystem<IAudioSystem>().PlaySound(
                "Assets/AssetBundle/Sounds/UI/ESM_Battle_Game_UI_Interface_Menu_Clip_Button_Smooth_Warm_Soft_1_Hybrid_One_Shot 1.wav");

        }

        private void ConnectServer(string ip)
        {
            NetworkService.Instance.serverIp = ip.Trim();
            NetworkService.Instance.serverPort = 8888;
            NetworkService.Instance.isSinglePlayer = false;
            Listener.OnRoomInfoReceived -= OnDirectJoinRoomInfo;   
            // 防重复订阅
            Listener.OnRoomInfoReceived += OnDirectJoinRoomInfo;   
            NetworkService.Instance.ConnectToServer();

        }

        private void OnDirectJoinRoomInfo(RoomInfoDto info)
        {
            Debug.Log("初始化房间");
            Listener.OnRoomInfoReceived -= OnDirectJoinRoomInfo;
            OnJoinRoom?.Invoke(info);
        }
        
        private void OnDisable()
        {
            Listener.OnRoomInfoReceived -= OnDirectJoinRoomInfo;   
        }


        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}