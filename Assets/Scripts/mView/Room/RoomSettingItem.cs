using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace mView.Room
{
    public class RoomSettingItem : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI value;

        public void Init(Sprite sprite, string str = "")
        {
            icon.sprite = sprite;
            value.text = str;
        }
    }
}