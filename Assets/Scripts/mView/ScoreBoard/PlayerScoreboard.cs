using TMPro;
using UnityEngine;

namespace mView.ScoreBoard
{
    public class PlayerScoreboard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private TextMeshProUGUI kill;
        [SerializeField] private TextMeshProUGUI death;

        public void Init(string nameStr, int killVal, int deathVal)
        {
            playerName.text = nameStr;
            kill.text = killVal.ToString();
            death.text = deathVal.ToString();
            gameObject.SetActive(true);
        }
    }
}