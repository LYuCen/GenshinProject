using DG.Tweening;
using mModel;
using mSystem.BuffSystem.ElementBuff;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace mView
{
    public class WeaponInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gunName;
        [SerializeField] private TextMeshProUGUI bulletText;
        [SerializeField] private Image weaponElementImg;
        [SerializeField] private Image grenadeElementImg;
        [SerializeField] private Image[] elementBulletImg;
        [SerializeField] private Image[] elementIconImg;
        [SerializeField] private TextMeshProUGUI[] elementBullets;
        [SerializeField] private RectTransform[] weapon;
        
        
        [SerializeField] private RectTransform mainRt;      // 主武器 UI
        [SerializeField] private RectTransform auxiliaryRt; // 附武器 UI 

        private ElementEnum[] _ints = new ElementEnum[3];

        private int _prevIndex;
        private int _useWeaponIndex;

        private void Start()
        {
            _useWeaponIndex = 0; // 默认 0 号下标为初始武器
        }

        public void Init(int[] bullets)
        {
            int j = 0;
            for (int i = 0; i < 8; i++)
            {
                if (bullets[i] > 0 && j < 3)
                {
                    elementBullets[j].text = $"{bullets[i]:D3}";
                    elementIconImg[j].sprite = ElementModel.Instance.elementUI[i];
                    _ints[j] = (ElementEnum)i;
                    j++;
                }
            }
        }

        /// <summary>
        /// 更新子弹数
        /// </summary>
        /// <param name="bullets">备弹数组</param>
        public void UpdateBulletsText(int[] bullets)
        {
            for (int i = 0; i < 3; i++)
            {
                elementBullets[i].text = $"{bullets[(int)_ints[i]]:D3}";
            }
        }
        
        /// <summary>
        /// 更新当前枪械弹匣子弹数
        /// </summary>
        /// <param name="bulletNumber">当前弹匣子弹</param>
        /// <param name="allBulletNumber">所有子弹</param>
        public void UpdateText(int bulletNumber, int allBulletNumber)
        {
            bulletText.text =
                $"{bulletNumber:D3}/{allBulletNumber:D3}";
        }

        /// <summary>
        /// 更新武器
        /// </summary>
        /// <param name="str">武器名称</param>
        /// <param name="index">武器下标</param>
        public void UpdateWeapon(string str,int index)
        {
            if (index == _useWeaponIndex || index > 1) return;
            gunName.text = str;

            RectTransform prevRt = weapon[_useWeaponIndex];
            prevRt.SetParent(auxiliaryRt);
            prevRt.gameObject.SetActive(false);
            prevRt.localScale /= 4;
            RectTransform rt = weapon[index];
            rt.SetParent(mainRt);
            
            Sequence seq = DOTween.Sequence();

            // 同时进行（Join）
            seq.Join(rt.DOAnchorPos(new Vector2(0, 0), 0.6f).SetEase(Ease.OutCubic));
            seq.Join(rt.DOScale(4f, 0.6f));

            // 播放完后回调
            seq.OnComplete(() =>
            {
                Debug.Log("切换结束");
                prevRt.anchoredPosition = Vector2.zero;
                prevRt.gameObject.SetActive(true);
                _useWeaponIndex = index;
            });
        }

        /// <summary>
        /// 切换元素
        /// </summary>
        /// <param name="add">偏移量 （土1）</param>
        public void UpdateElementBullets(int add)
        {
            elementBulletImg[_prevIndex].color = new Color(0.721f, 0.721f, 0.721f, 0.7f);
            _prevIndex = (_prevIndex + add + 3) % 3;
            elementBulletImg[_prevIndex].color = new Color(0.866f, 0.714f, 0.282f, 0.7f);
        }

        /// <summary>
        /// 获取当前选择元素
        /// </summary>
        /// <returns>选择下标</returns>
        public ElementEnum GetChose()
        {
            return _ints[_prevIndex];
        }
        
        /// <summary>
        ///  更新武器元素 UI
        /// </summary>
        /// <param name="index">元素下标</param>
        public void ElementImage(int index)
        { 
            weaponElementImg.sprite = ElementModel.Instance.elementUI[index];
        }

        /// <summary>
        /// 更新投掷物元素UI
        /// </summary>
        /// <param name="index">元素下标</param>
        public void GrenadeElementImage(int index)
        {
            grenadeElementImg.sprite = ElementModel.Instance.elementUI[index];
        }
        
    }
}