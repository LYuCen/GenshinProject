using UnityEngine;
using UnityEngine.UI;

namespace mView.Buff
{
    public class ElementUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image backGround;
        
        public void SetIcon(Sprite ui)
        {
            icon.sprite = ui;
            backGround.sprite = ui;
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