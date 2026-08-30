using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using AssetBundleFramework.Core.Resource;
using Connect;
using Connect.DTO;
using Connect.Protocol;
using mCommand;
using mSystem.SceneSystem;
using mView.Button;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace mView.Room.RoomSetting
{
    public class OrganizeRoomView : MonoBehaviour, IController
    {
        [SerializeField] private BaseButton backBtn;
        [SerializeField] private BaseButton startGameBtn;
        [SerializeField] private Transform playerContent;

        [SerializeField] private Image gameSceneImg;
        [SerializeField] private TextMeshProUGUI sceneName;
        [SerializeField] private TextMeshProUGUI roomName;
        [SerializeField] private TextMeshProUGUI roomIp;
        [SerializeField] private TextMeshProUGUI roomIntroduction;
        [SerializeField] private Transform roomSettingContent;

        private bool _isHost;
        private int _roomMaxPlayer;
        private int _roomTime;
        private int _roomKill;
        public void Init(CreateRoomSettingData data, int playMax, int time, int kill)
        {
            Populate(data.sceneName, data.roomName, data.roomIntroduction, GetLocalIP(),
                data.roomPassword.Length > 0, playMax, time, kill);
            startGameBtn.gameObject.SetActive(true);

            _roomMaxPlayer = playMax;
            _roomTime = time;
            _roomKill = kill;
            _isHost = true;
        }
        
        public void Init(RoomInfoDto info)
        {
            Populate(info.sceneName, info.roomName, info.roomIntroduction,
                info.hostIp, info.hasPassword, info.maxPlayers, info.gameTime, info.killValue);
            startGameBtn.gameObject.SetActive(false);
            
            _roomMaxPlayer = info.maxPlayers;
            _roomTime = info.gameTime;
            _roomKill = info.killValue;
            _isHost = false;
        }
        
        public void Init(RoomInfoDto info, bool isHost)
        {
            Populate(info.sceneName, info.roomName, info.roomIntroduction,
                info.hostIp, info.hasPassword, info.maxPlayers, info.gameTime, info.killValue);
            startGameBtn.gameObject.SetActive(isHost);
            
            _roomMaxPlayer = info.maxPlayers;
            _roomTime = info.gameTime;
            _roomKill = info.killValue;
            _isHost = isHost;
        }

        private void OnEnable()
        {
            Listener.OnLobbyPlayerReceived += OnLobbyPlayers;
            NetworkService.OnServerLost += OnServerLost;
            RequestPlayerList();
        }

        

        private void Populate(string scene, string rname, string intro, string ip,
            bool hasPassword, int maxPlayers, int time, int kill)
        {
            sceneName.text = scene;
            roomName.text = rname;
            roomIntroduction.text = intro;
            roomIp.text = GetLocalIP();

            if (hasPassword)
            {
                var obj = RoomSettingObjPool.Instance.GetObj().GetComponent<RoomSettingItem>();
                var ir = ResourceManager.instance.Load("Assets/AssetBundle/Icon/UIIcon/UI_Icon_Lock.png", false);
                obj.Init(ir.GetAsset<Sprite>());
                obj.transform.SetParent(roomSettingContent, false);
            }
            
            var rti = RoomSettingObjPool.Instance.GetObj().GetComponent<RoomSettingItem>();
            rti.Init(
                ResourceManager.instance.Load("Assets/AssetBundle/Icon/UIIcon/UI_Icon_Player.png", false).GetAsset<Sprite>(),
                maxPlayers.ToString());
            rti.transform.SetParent(roomSettingContent, false);
            
            rti = RoomSettingObjPool.Instance.GetObj().GetComponent<RoomSettingItem>();
            rti.Init(
                ResourceManager.instance.Load("Assets/AssetBundle/Icon/UIIcon/UI_Icon_Time.png", false).GetAsset<Sprite>(),
                time.ToString());
            rti.transform.SetParent(roomSettingContent, false);
            
            rti = RoomSettingObjPool.Instance.GetObj().GetComponent<RoomSettingItem>();
            rti.Init(
                ResourceManager.instance.Load("Assets/AssetBundle/Icon/UIIcon/UI_Icon_Death.png", false).GetAsset<Sprite>(),
                kill.ToString());
            rti.transform.SetParent(roomSettingContent, false);
        }

        private void Start()
        {
            backBtn.OnClick = () =>
            {
                string tip = _isHost
                    ? "关闭房间将踢出所有玩家，是否关闭？"
                    : "确定离开房间吗？";

                this.SendCommand(new TextTipCommand(tip,
                    () =>
                    {
                        NetworkService.Instance.StateMachine.Disconnect();
                        if (_isHost) DedicatedServerLauncher.Instance.Kill();
                        gameObject.SetActive(false);
                    }, null, null));
            };
            startGameBtn.OnClick += StartGame;
        }

        private void StartGame()
        {
            Debug.Log(
                $"[StartGame] Bridge={(NetworkService.Bridge == null ? "null" : "非null")} MyId={NetworkService.Bridge?.GetMyId()}");
            NetworkService.Bridge?.SendToServer(new Message(MessageType.C2S_StartGame, Array.Empty<byte>()));
        }


        private void OnDisable()
        {
            for (int i = roomSettingContent.childCount - 1; i >= 0; i--)
            {
                RoomSettingObjPool.Instance.RemoveObj(roomSettingContent.GetChild(i).gameObject);
            }

            Listener.OnLobbyPlayerReceived -= OnLobbyPlayers;
            NetworkService.OnServerLost -= OnServerLost;
        }

        private void OnServerLost()
        {
            this.SendCommand(new TextTipCommand("服务器连接超时", null, null,
                () => this.GetSystem<ISceneSystem>().LoadSceneAsync("Lobby")));
        }

        private void RequestPlayerList()
        {
            NetworkService.Bridge?.SendToServer(new Message(MessageType.C2S_GetLobbyPlayers, Array.Empty<byte>()));
        }

        private void OnLobbyPlayers(List<LobbyPlayerDto> players)
        {
            for (int i = 0; i < playerContent.childCount; i++)
            {
                var info = playerContent.GetChild(i).GetComponent<RoomPlayerInfo>();
                if(info == null) continue;
                info.Reset();
                info.gameObject.SetActive(true);
                if (i < players.Count) info.Init(players[i].name);
                else if(i >= _roomMaxPlayer) info.gameObject.SetActive(false);
            }
        }


        private static string GetLocalIP()
        {
            string hostName = Dns.GetHostName();

            foreach (var ip in Dns.GetHostAddresses(hostName))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}