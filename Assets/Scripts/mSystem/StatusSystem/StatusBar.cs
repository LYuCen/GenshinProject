using UnityEngine;
using UnityEngine.UI;

namespace mSystem.StatusSystem
{
    public class StatusBar
    {
        public float FullBar;   // 满条
        public float RealBar;   // 实际状态条
        private float _maniBar;   // manifest 表现条
        
        private Image _barImg;    // 基础图片
        private Image _loseBarImg;// 数值减少条 (血条减少时的白条)
        private Image _riseBarImg;// 数值增长条 (回血增加时候的条)

        public float TransitionSpeed; // 状态条过渡速度

        public StatusBar(float fullBar, Image barImg, Image loseBarImg, Image riseBarImg, float speed = 40f)
        {
            FullBar = fullBar;
            RealBar = fullBar;
            _maniBar = fullBar;
            _barImg = barImg;
            _loseBarImg = loseBarImg;
            _riseBarImg = riseBarImg;
            TransitionSpeed = speed;
        }

        public void OnUpdate()
        {
            _maniBar = Mathf.MoveTowards(_maniBar, RealBar, TransitionSpeed * Time.deltaTime);
            
            float realRatio = RealBar / FullBar;
            float maniRatio = _maniBar / FullBar;

            if (RealBar < _maniBar) 
            {
                // --- 掉血状态 ---
                _barImg.fillAmount = realRatio;       // 主条瞬间减短
                _loseBarImg.fillAmount = maniRatio;   // 掉血条（白条）显示表现值，慢慢减少
                _riseBarImg.fillAmount = realRatio;   // 增长条隐藏在主条下
            }
            else if (RealBar > _maniBar)
            {
                // --- 回血状态 ---
                _barImg.fillAmount = maniRatio;       // 主条显示表现值，慢慢增长
                _loseBarImg.fillAmount = maniRatio;   // 掉血条隐藏在主条下
                _riseBarImg.fillAmount = realRatio;   // 增长条瞬间变长，作为背景
            }
            else
            {
                // --- 完全相等状态 ---
                _barImg.fillAmount = realRatio;
                _loseBarImg.fillAmount = realRatio;
                _riseBarImg.fillAmount = realRatio;
            }
        }
    }
}