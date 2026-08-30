using System;
using System.Collections;
using AssetBundleFramework.Core.Resource;
using Connect;
using Connect.Converter;
using Connect.Core;
using Connect.DTO;
using Connect.Protocol;
using mView.Button;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace mView.Room.RoomSetting
{
    public class CreateRoomView : MonoBehaviour
    {
        [SerializeField] private CreateRoomSetting roomSetting;
        [SerializeField] private BaseButton closeBtn;

        public Action<CreateRoomSettingData, int, int, int> OnCreateRoom;
        
        private void OnEnable()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/ScriptableObject/Setting/Room/DeathCross.asset",
                false);
            roomSetting.Init(ir.GetAsset<CreateRoomSettingData>());
            
        }

        public void Start()
        {
            roomSetting.CreateBtnClick = CreateRoom;
            closeBtn.OnClick = () => gameObject.SetActive(false);
        }

        private void CreateRoom(CreateRoomSettingData data, int playValue, int time, int kill)
        {
            Debug.Log($"创建{data.name}房间，最大人数{playValue},时长{time},击杀数{kill}");
            StartCoroutine(CreateRoomRoutine(data, playValue, time, kill));
        }

        private IEnumerator CreateRoomRoutine(CreateRoomSettingData data, int playValue, int time, int kill)
        {
            NetworkService.Instance.isSinglePlayer = false;

            // 编辑器环境下自己开启服务器
            if (!Application.isEditor)
            {
                if (DedicatedServerLauncher.Instance.IsServerRunning())
                {
                    Debug.Log("检测到本地服务器运行，直接连接");
                }
                else
                {
                    DedicatedServerLauncher.Instance.Launch();
                }

                bool connected = false;
                for (int i = 0; i < 20; i++)
                {
                    NetworkService.Instance.ConnectToServer();
                    if (NetworkService.Instance.StateMachine.CurrentState == ConnectionState.Connected)
                    {
                        connected = true;
                        break;
                    }

                    yield return new WaitForSeconds(0.5f);
                }

                if (!connected)
                {
                    Debug.LogError("连接服务器失败");
                    DedicatedServerLauncher.Instance.Kill();
                    yield break;
                }
            }
            else
            {
                NetworkService.Instance.ConnectToServer();
            }
            
            var dto = new RoomInfoDto
            {
                roomName = data.roomName,
                sceneName = data.sceneName,
                roomIntroduction = data.roomIntroduction,
                hasPassword = !string.IsNullOrEmpty(data.roomPassword),
                maxPlayers = (ushort)playValue,
                gameTime = (ushort)time,
                killValue = (ushort)kill
            };

            NetworkService.Bridge?.SendToServer(
                new Message(MessageType.C2S_CreateRoom, RoomInfoConverter.PackInfo(dto)));
            
            OnCreateRoom?.Invoke(data, playValue, time, kill);
        }
    }
}