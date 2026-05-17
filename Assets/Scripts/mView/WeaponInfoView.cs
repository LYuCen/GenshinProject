using DG.Tweening;
using Firearms;
using mEvent;
using mModel;
using mSystem.BuffSystem.ElementBuff;
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

        private ElementEnum[] _ints = new ElementEnum[3];
        private BagModel _bagModel;
        private AbstractGun _gun;

        private int _prevIndex;
        private int _useWeaponIndex;

        private void Start()
        {
            _useWeaponIndex = 0; // 默认 0 号下标为初始武器
            _bagModel = this.GetModel<BagModel>();
            this.RegisterEvent<ViewUpdateWeaponEvent>(e =>
            {
                UpdateWeapon(e.Index);
                _gun = e.Index == 0 ? _bagModel.GetPrimaryWeapon() : _bagModel.GetSecondaryWeapon();
                weaponElementImg.sprite = ElementModel.Instance.elementUI[(int)_gun.Magazine.Element];
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<ViewUpdateElement>(e =>
            {
                UpdateElementBullets(e.Offset);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<ViewInitEvent>(_ =>
                {
                    InitUI();
                    _gun = _bagModel.GetPrimaryWeapon();
                })
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<ShootEvent>(_ => UpdateText())
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<ReloadEndEvent>(_ =>
            {
                UpdateText();
                weaponElementImg.sprite = ElementModel.Instance.elementUI[(int)_gun.Magazine.Element];
                InitUI();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<EnterShootEvent>(_ => EnableFrontSight(false))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<ExitShootEvent>(_ => EnableFrontSight(true))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void InitUI()
        {
            for (int i = 0; i < 3; i++)
            {
                var bullet = _bagModel.GetBullet(i);
                elementIconImg[i].sprite = ElementModel.Instance.elementUI[(int)bullet.Data];
                elementBullets[i].text = $"{bullet.Val:D3}";
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
        private void UpdateText()
        {
            bulletText.text =
                $"{_gun.Magazine.BulletNumber:D3}/{_bagModel.GetBullet(_gun.Magazine.Element).Val:D3}";
        }

        /// <summary>
        /// 更新武器
        /// </summary>
        /// <param name="index">武器下标</param>
        private void UpdateWeapon(int index)
        {
            if (index == _useWeaponIndex || index > 1) return;

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
        /// <param name="enable"></param>
        public void EnableFrontSight(bool enable)
        {
            frontSight.SetActive(enable);
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}