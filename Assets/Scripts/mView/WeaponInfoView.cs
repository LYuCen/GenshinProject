using DG.Tweening;
using mEvent;
using mModel;
using mWeapon;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace mView
{
    public class WeaponInfoView : MonoBehaviour, IController
    {
        [SerializeField] private TextMeshProUGUI gunName;
        [SerializeField] private TextMeshProUGUI bulletText;
        [SerializeField] private Image weaponElementImg;
        [SerializeField] private Image grenadeElementImg;
        [SerializeField] private Image[] elementBulletImg;
        [SerializeField] private Image[] elementIconImg;
        [SerializeField] private TextMeshProUGUI[] elementBullets;
        [SerializeField] private RectTransform[] weapon;

        [SerializeField] private GameObject frontSight;
        [SerializeField] private RectTransform mainRt;      // 主武器 UI
        [SerializeField] private RectTransform auxiliaryRt; // 附武器 UI 
        
        private BagModel _bagModel;

        private int _prevIndex;
        private int _useWeaponIndex;

        private void Start()
        {
            _useWeaponIndex = 0; // 默认 0 号下标为初始武器
            _bagModel = this.GetModel<BagModel>();
            this.RegisterEvent<ViewUpdateWeaponEvent>(e =>
            {
                UpdateWeapon(e.Index);
                weaponElementImg.sprite =
                    ElementModel.Instance.elementUI[(int)_bagModel.Weapon.Runtime.Component.ElementEnum];
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<ViewUpdateElement>(e =>
            {
                UpdateElementBullets(e.Offset);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<WeaponDataUpdateEvent>(_ => UpdateUI())
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<FrontSightEvent>(_ =>
                EnableFrontSight(_bagModel.Weapon.Runtime.Stance != WeaponStance.Ads))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void UpdateUI()
        {
            for (int i = 0; i < 3; i++)
            {
                var bullet = _bagModel.GetBullet(i);
                elementIconImg[i].sprite = ElementModel.Instance.elementUI[(int)bullet.Data];
                elementBullets[i].text = $"{bullet.Val:D3}";
            }

            gunName.text = _bagModel.Weapon.WeaponName;
            bulletText.text =
                $"{_bagModel.Weapon.Runtime.Component.BulletVal:D3}/" +
                $"{_bagModel.GetBullet(_bagModel.Weapon.Runtime.Component.ElementEnum).Val:D3}";
            weaponElementImg.sprite =
                ElementModel.Instance.elementUI[(int)_bagModel.Weapon.Runtime.Component.ElementEnum];
        }
            

        /// <summary>
        /// 更新武器：次武器从副框位置+大小，过渡到主框位置+大小
        /// </summary>
        /// <param name="index">武器下标</param>
        private void UpdateWeapon(int index)
        {
            if (index == _useWeaponIndex || index > 1) return;

            RectTransform prevRt = weapon[_useWeaponIndex]; // 当前主武器
            RectTransform nextRt = weapon[index];            // 要切换的武器（在副框）

            // 计算两个框中心的世界坐标偏移
            Vector3 offset = auxiliaryRt.position - mainRt.position;

            // 新武器移到主框：拉伸锚点填满，初始缩放到副框大小，偏移到副框位置
            nextRt.SetParent(mainRt);
            SetStretchFill(nextRt);
            nextRt.localScale = Vector3.one / 3.33f;
            nextRt.localPosition = offset;

            Image prevImg = prevRt.GetComponent<Image>();

            Sequence seq = DOTween.Sequence();

            // 从小变大 + 从副框位置移到主框中心
            seq.Join(nextRt.DOScale(1f, 0.6f).SetEase(Ease.OutCubic));
            seq.Join(nextRt.DOLocalMove(Vector3.zero, 0.6f).SetEase(Ease.OutCubic));
            // 主武器淡出
            if (prevImg != null)
                seq.Join(prevImg.DOFade(0f, 0.4f));

            seq.OnComplete(() =>
            {
                // 旧主武器移到副框：拉伸填满副框，恢复大小和位置
                prevRt.SetParent(auxiliaryRt);
                SetStretchFill(prevRt);
                prevRt.localScale = Vector3.one;
                prevRt.localPosition = Vector3.zero;
                if (prevImg)
                    prevImg.DOFade(1f, 0.2f);

                _useWeaponIndex = index;
            });
        }

        /// <summary>
        /// 设置 RectTransform 锚点拉伸填满父节点
        /// </summary>
        private static void SetStretchFill(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// 切换元素
        /// </summary>
        /// <param name="add">偏移量 （土1）</param>
        private void UpdateElementBullets(int add)
        {
            elementBulletImg[_prevIndex].color = new Color(0.721f, 0.721f, 0.721f, 0.7f);
            _prevIndex = (_prevIndex + add + 3) % 3;
            elementBulletImg[_prevIndex].color = new Color(0.866f, 0.714f, 0.282f, 0.7f);
        }

        /// <summary>
        /// 更新投掷物元素UI
        /// </summary>
        /// <param name="index">元素下标</param>
        public void GrenadeElementImage(int index)
        {
            grenadeElementImg.sprite = ElementModel.Instance.elementUI[index];
        }

        /// <summary>
        /// 准星是否可见
        /// </summary>
        private void EnableFrontSight(bool enable)
        {
            frontSight.SetActive(enable);
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}