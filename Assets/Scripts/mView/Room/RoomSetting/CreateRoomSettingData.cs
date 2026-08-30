using System.Collections.Generic;
using UnityEngine;

namespace mView.Room.RoomSetting
{
    [CreateAssetMenu(menuName = "Config/CreateRoomSettingData")]
    public class CreateRoomSettingData : ScriptableObject
    {
        public Sprite roomSprite;
        public string sceneName;
        public string roomName;
        public string roomIntroduction;
        public string roomPassword;
        public List<int> playerValue;
        public List<int> gameTime;
        public List<int> killValue;
    }
}