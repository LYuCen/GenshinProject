using System;
using Connect;
using Connect.DTO;
using Connect.Protocol;
using mSystem.InputSystem;
using mView.Room.RoomSetting;
using UnityEngine;

namespace mView.Room
{
    public class LobbyRoomView : MonoBehaviour
    {
        [SerializeField] private CreateRoomView createRoomView;
        [SerializeField] private OrganizeRoomView organizeRoomView;
        [SerializeField] private JoinRoomView joinRoomView;

        private void Start()
        {
            createRoomView.OnCreateRoom = InitOrganizeRoom;
            joinRoomView.OnJoinRoom = InitOrganizeRoom;
            
            if (GameEnter.Instance != null && GameEnter.Instance.returnFromGame)
            {
                GameEnter.Instance.returnFromGame = false;  
                Listener.OnRoomInfoReceived += OnReturnRoomInfo;
                NetworkService.Bridge?.SendToServer(new Message(MessageType.C2S_GetRoomInfo, Array.Empty<byte>()));
            }
        }
        
        private void OnReturnRoomInfo(RoomInfoDto dto)      
        {
            Listener.OnRoomInfoReceived -= OnReturnRoomInfo;
            organizeRoomView.Init(dto, GameEnter.Instance.returnIsHost);
            organizeRoomView.gameObject.SetActive(true);    
            createRoomView.gameObject.SetActive(false);     
            joinRoomView.gameObject.SetActive(false);       
        }



        private void InitOrganizeRoom(CreateRoomSettingData data, int playValue, int time, int kill)
        {
            organizeRoomView.Init(data, playValue, time, kill);
            organizeRoomView.gameObject.SetActive(true);
            createRoomView.gameObject.SetActive(false);
        }

        private void InitOrganizeRoom(RoomInfoDto dto)
        {
            organizeRoomView.Init(dto);
            organizeRoomView.gameObject.SetActive(true);
            joinRoomView.gameObject.SetActive(false);
        }

        public void ShowCreateRoomView()
        {
            createRoomView.gameObject.SetActive(true);
        }

        public void ShowJoinRoomView()
        {
            joinRoomView.gameObject.SetActive(true);
        }
    }
}