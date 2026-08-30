using System;
using Connect;
using Connect.DTO;
using mCommand;
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
            closeBtn.OnClick = () => gameObject.SetActive(false);
        }

        private void DirectConnect()
        {
            this.SendCommand(new InputTipCommand("IP直连", "请输入房间IP地址", ConnectServer, null));
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