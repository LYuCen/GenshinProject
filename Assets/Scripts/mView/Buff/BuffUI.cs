using UnityEngine;
using UnityEngine.UI;

namespace mView.Buff
{
    public class BuffUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;

        public void SetIcon(Sprite ui)
        {
            icon.sprite = ui;
            background.sprite = ui;
        }

        public void SetProgress(float progress)
        {
            icon.fillAmount = progress;
        }

        public float GetProgress()
        {
            return icon.fillAmount;
        }
    }
}