using Firearms;
using UnityEngine;

namespace Player.AnimPro
{
    public abstract class AnimTreeLeafNode<T> : AnimTreeNode<T>
    {
        public WeaponClipAsset CurrentData;
        
        protected AnimTreeLeafNode(T myMessage) : base(myMessage)
        {
        }

        public override void Evaluate(bool parentActive = true)
        {
            // 判定自身逻辑是否满足
            bool conditionMet = CheckState(true);
            // 判定自身是否应该作为“目标”激活
            bool selfActive = parentActive && conditionMet;
            
            TargetWeight = selfActive ? 1f : 0f;
        }
        
        public override float UpdateNode()
        {
            Weight = Mathf.MoveTowards(Weight, TargetWeight, Time.fixedDeltaTime * WeightSpeed);
            return Weight;
        }
    }
}