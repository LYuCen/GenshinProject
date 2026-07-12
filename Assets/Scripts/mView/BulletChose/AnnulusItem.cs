using UnityEngine;
using UnityEngine.UI;

namespace mView.BulletChose
{
    public class AnnulusItem : MonoBehaviour
    {
        [SerializeField] private Image img;
        [SerializeField] private Image highImg;
        [SerializeField] private RectTransform rt;
        [SerializeField] private RectTransform highLight;

        public void SetFillAmount(float val)
        {
            img.fillAmount = val;
            highImg.fillAmount = val;
        }

        public void SetRotationZ(float val)
        {
            rt.rotation = Quaternion.Euler(0, 0, val);
            highLight.rotation = Quaternion.Euler(0, 0, val);
        }

        public void SetHighLight(bool on)
        {
            highLight.gameObject.SetActive(on);
        }
    }
}