using System;
using Connect.DTO;
using mView.Button;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace mView.Room
{
    public class RoomSceneItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject enter;
        [SerializeField] private TextMeshProUGUI sceneName;
        [SerializeField] private TextMeshProUGUI roomName;
        [SerializeField] private TextMeshProUGUI createName;
        [SerializeField] private GameObject roomSetting;
        [SerializeField] private TextMeshProUGUI onlineValue;
        [SerializeField] private TextMeshProUGUI ping;
        [SerializeField] private BaseButton btn;

        private RoomInfoDto _info;
        public Action<RoomInfoDto> OnClick;

        public void Init(RoomInfoDto info)
        {
            _info = info;
            sceneName.text = info.sceneName;
            roomName.text = info.roomName;
            createName.text = info.hostIp;
            onlineValue.text = $"{info.currentPlayers}/{info.maxPlayers}";
            ping.text = "--";
        }

        private void Start()
        {
            btn.OnClick = () => OnClick?.Invoke(_info);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            enter.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            enter.SetActive(false);
        }
    }
}