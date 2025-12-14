using UnityEngine;

namespace Firearms
{
    public class AbstractThrow
    {
        protected AbstractMagazine Magazine; // 弹匣
        protected Vector3 Forward; // 正方向
        protected Transform Transform; // 投掷物 Transform
        protected Transform MagazineTransform; // 弹匣 Transform
        protected float MainRadius; // 伤害半径 (中心)
        protected float OtherRadius; // 波及半径 (会递减)
        protected float BoomCountDown; // 爆炸倒计时 
        protected bool IsReadyThrow; // 是否拉环
        protected bool IsThrow; // 是否投掷
        protected bool IsDownThrow; // 是否低抛 (默认右键切换高低抛)
        protected bool Enable = true; // 是否激活武器

        public bool GetIsReadyThrow()
        {
            return IsReadyThrow;
        }

        public bool GetIsDownThrow()
        {
            return IsDownThrow;
        }

        public bool GetIsThrow()
        {
            return IsThrow;
        }
    }
}