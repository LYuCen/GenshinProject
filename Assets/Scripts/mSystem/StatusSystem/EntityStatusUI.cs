using Connect.Data;
using UnityEngine;
using UnityEngine.UI;

namespace mSystem.StatusSystem
{
    public class EntityStatusUI : MonoBehaviour
    {
        [Header("血条")]
        [SerializeField] [Range(0, 200)] private float maxHp;
        [SerializeField] private Image hpBar;
        [SerializeField] private Image hpLoseBar;
        [SerializeField] private Image hpRiseBar;

        [Header("盾条")]
        [SerializeField] [Range(0, 200)] private float maxShield;
        [SerializeField] private Image shieldBar;
        [SerializeField] private Image shieldLoseBar;
        [SerializeField] private Image shieldRiseBar;

        private StatusBar _hpStatusBar;
        private StatusBar _shieldStatusBar;

        [Header("元素图标")] [SerializeField] private Transform elementBox;

        public void Start()
        {
            Canvas canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main; 
            canvas.planeDistance = 1.0f; // 贴近相机
        }

        public void InitStatus(ref PlayerStateData state)
        {
            maxHp = state.maxHp;
            maxShield = state.maxShield;
            _hpStatusBar = new StatusBar(maxHp, hpBar, hpLoseBar, hpRiseBar, 50);
            _shieldStatusBar = new StatusBar(maxShield, shieldBar, shieldLoseBar, shieldRiseBar, 30);
        }
        
        public void SyncValues(float hp, float mHp, float shield, float mShield)
        {
            // 更新上限（防止升级或换装后上限改变）
            maxHp = mHp;
            maxShield = mShield;
            _hpStatusBar.FullBar = mHp;
            _shieldStatusBar.FullBar = mShield;

            // 设置目标实际值
            _hpStatusBar.RealBar = hp;
            _shieldStatusBar.RealBar = shield;
        }

        /// <summary>
        /// 添加元素图标
        /// </summary>
        public void AddElement(Transform ui)
        {
            
            ui.SetParent(elementBox);
            
        }

        public void Update()
        {
            _hpStatusBar.OnUpdate();
            _shieldStatusBar.OnUpdate();
        }

        public void SetHp(float newHp)
        {
            _hpStatusBar.RealBar = newHp;
        }

        public void SetShield(float newShield)
        {
            _shieldStatusBar.RealBar = newShield;
        }
    }
}