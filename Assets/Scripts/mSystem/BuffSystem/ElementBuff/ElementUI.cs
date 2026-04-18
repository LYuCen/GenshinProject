using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace mSystem.BuffSystem.ElementBuff
{
    public class ElementUI: MonoBehaviour
    {
        [SerializeField] private Image baseUI;
        [SerializeField] private Image outUI;
        [SerializeField] private RectTransform baseRt;
        [SerializeField] private RectTransform rt;

        private Tweener _tween;
        private bool _willEnd;

        public void Init(Sprite sprite)
        {
            baseUI.sprite = sprite;
            outUI.sprite = sprite;
            outUI.gameObject.SetActive(false);
            baseRt.localScale = Vector3.one;
            baseRt.localPosition = Vector3.zero;
            baseRt.localRotation = new Quaternion(0, 0, 0, 0);
        }

        public void ShowOutUI()
        {
            outUI.gameObject.SetActive(true);
            rt.sizeDelta = new Vector2(200, 200);
            rt.DOSizeDelta(new Vector2(300, 300), 0.75f)
                .OnComplete(() => outUI.gameObject.SetActive(false));
        }

        public void WillLose(float time)
        {
            if (time > 3f)
            {
                _willEnd = false;
                Color c = baseUI.color;
                c.a = 0.8f;
                baseUI.color = c;
                _tween?.Kill();
            }
            else
            {
                if (_willEnd) return;
                Color c = baseUI.color;
                c.a = 0.8f;
                _tween = DOTween.To(() => c.a, x =>
                    {
                        c.a = x;
                        baseUI.color = c;
                    }, 0.3f, 0.75f)
                    .SetLoops(-1, LoopType.Yoyo);
                _willEnd = true;
            }
            
        }

        public void OnDisable()
        {
            _tween?.Kill();
        }
    }
}