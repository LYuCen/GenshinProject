using mView.Button;
using TMPro;
using UnityEngine;

namespace mView.Room
{
    public class RoomPlayerInfo : MonoBehaviour
    {
        [SerializeField] private GameObject addPlayer;
        [SerializeField] private GameObject headIcon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private BaseButton removeBtn;

        private void Start()
        {
            removeBtn.OnClick = () => { Debug.Log($"移除玩家 {nameText.text}"); };
        }

        public void Init(string nameStr)
        {
            nameText.text = nameStr;
            headIcon.SetActive(true);
            nameText.gameObject.SetActive(true);
            addPlayer.SetActive(false);
        }

        public void Reset()
        {
            headIcon.SetActive(false);
            nameText.gameObject.SetActive(false);
            addPlayer.SetActive(true);
        }
    }
}