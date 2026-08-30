using System;
using mCommand;
using mView.Button;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace mView.Room.RoomSetting
{
    public class CreateRoomSetting : MonoBehaviour, IController
    {
        [SerializeField] private Image roomSceneImage;
        [SerializeField] private TextMeshProUGUI roomName;
        [SerializeField] private TextMeshProUGUI roomIntroduction;
        [SerializeField] private TextMeshProUGUI roomPassword;
        [SerializeField] private SettingTextButton roomMaxValuePlayer;
        [SerializeField] private SettingTextButton roomGameTime;
        [SerializeField] private SettingTextButton roomKillValue;
        [SerializeField] private BaseButton roomNameBtn;
        [SerializeField] private BaseButton roomIntroductionBtn;
        [SerializeField] private BaseButton roomPasswordBtn;
        [SerializeField] private BaseButton createBtn;
        
        private CreateRoomSettingData _data;
        private int _maxValueIndex;
        private int _gameTimeIndex;
        private int _killValueIndex;
        public Action<CreateRoomSettingData, int, int, int> CreateBtnClick;

        private void Start()
        {
            roomMaxValuePlayer.OnClick += SetMaxValue;
            roomGameTime.OnClick += SetGameTimeValue;
            roomKillValue.OnClick += SetKillValue;

            roomNameBtn.OnClick += () => this.SendCommand(new InputTipCommand("提示", "输入新的房间名称", SetRoomName, null));
            roomIntroductionBtn.OnClick +=
                () => this.SendCommand(new InputTipCommand("提示", "输入新的房间介绍", SetRoomIntroduction, null));
            roomPasswordBtn.OnClick += () => this.SendCommand(new InputTipCommand("提示", "输入房间密码", SetPassword, null));

            createBtn.OnClick += () => CreateBtnClick?.Invoke(_data, _data.playerValue[_maxValueIndex],
                _data.gameTime[_gameTimeIndex], _data.killValue[_killValueIndex]);
        }

        public void Init(CreateRoomSettingData data)
        {
            _maxValueIndex = 0;
            _gameTimeIndex = 0;
            _killValueIndex = 0;

            _data = Instantiate(data);
            roomSceneImage.sprite = data.roomSprite;
            roomName.text = data.roomName;
            roomIntroduction.text = data.roomIntroduction;
            roomPassword.text = data.roomPassword;
            roomMaxValuePlayer.SetText(data.playerValue[_maxValueIndex].ToString());
            roomGameTime.SetText(data.gameTime[_gameTimeIndex].ToString());
            roomKillValue.SetText(data.killValue[_killValueIndex].ToString());
        }

        private void SetRoomName(string str)
        {
            roomName.text = str;
            _data.roomName = str;
        }

        private void SetRoomIntroduction(string str)
        {
            roomIntroduction.text = str;
            _data.roomIntroduction = str;
        }

        private void SetPassword(string str)
        {
            roomPassword.text = str;
            _data.roomPassword = str;
        }

        private void SetMaxValue(int offset)
        {
            _maxValueIndex = (_maxValueIndex + offset + _data.playerValue.Count) % _data.playerValue.Count;
            roomMaxValuePlayer.SetText(_data.playerValue[_maxValueIndex].ToString());
        }

        private void SetGameTimeValue(int offset)
        {
            _gameTimeIndex = (_gameTimeIndex + offset + _data.gameTime.Count) % _data.gameTime.Count;
            roomGameTime.SetText(_data.gameTime[_gameTimeIndex].ToString());
        }

        private void SetKillValue(int offset)
        {
            _killValueIndex = (_killValueIndex + offset + _data.killValue.Count) % _data.killValue.Count;
            roomKillValue.SetText(_data.killValue[_killValueIndex].ToString());
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}