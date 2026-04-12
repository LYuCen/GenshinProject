using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Playables;

namespace Player.AnimPro
{
    public abstract class AnimTreeNode<T>
    {
        protected bool IsLeaf;          // 自己是否为叶子节点
        public float Weight;            // 权重  
        public float WeightSpeed = 5f;  // 权重转移速度
        public float TargetWeight;      // 目标权重
        public int MixIndex;            // 在父节点下所处下标       
        protected List<AnimTreeNode<T>> Children = new(); 
        protected T MyMessage;                 
        public AnimTreeNode<T> Parent;
        protected AnimTreeNode<T> LastActiveChild; 

        public AnimTreeNode(T myMessage) => MyMessage = myMessage;

        protected void AddNode(AnimTreeNode<T> node)
        {
            node.Parent = this;
            Children.Add(node);
        }

        /// <summary>
        /// 更新目标权重
        /// </summary>
        /// <param name="parentActive">父节点是否激活</param>
        public virtual void Evaluate(bool parentActive = true)
        {
            // 判定自身逻辑是否满足
            bool conditionMet = CheckState(true);
            // 判定自身是否应该作为“目标”激活
            bool selfActive = parentActive && conditionMet;

            if (IsLeaf)
            {
                TargetWeight = selfActive ? 1f : 0f;
                return;
            }
            
            AnimTreeNode<T> selected = null;
            if (selfActive)
            {
                // 寻找当前满足条件的子节点
                foreach (var child in Children)
                {
                    if (child.CheckState(true))
                    {
                        selected = child;
                        break;
                    }
                }
            }

            /*
             * 当自己的条件不满足时权重却还没降为 0
             * 就继续保持最后满足的节点，防止姿态出现错误
             */
            if (selected == null && Weight > 0.001f)
            {
                selected = LastActiveChild;
            }

            // 如果实在没选出来，选第一个，防止 Mixer 为空
            //if (selected == null && Children.Count > 0) selected = Children[0];

            LastActiveChild = selected;

            // 递归子节点：
            // 这里非常重要：我们向下传递的 "childActive" 必须基于我们选中的 selected
            // 而不是仅仅基于 selfActive。
            foreach (var child in Children)
            {
                bool isThisChildTarget = (selected == child);
                
                // 如果父级不再激活自己，子节点的目标权重必须是 0
                // 但是！子节点的 Evaluate 必须知道它是否仍是“被选中维持姿态”的那一个
                if (child != null)
                {
                    child.Evaluate(selfActive && isThisChildTarget);

                    // 子节点的目标权重：只有当 selfActive 且它是被选中的那个才是 1
                    child.TargetWeight = (selfActive && isThisChildTarget) ? 1f : 0f;
                }
            }

            TargetWeight = selfActive ? 1f : 0f;
        }

        /// <summary>
        /// 更新节点权重
        /// </summary>
        /// <returns>返回当前权重</returns>
        public virtual float UpdateNode()
        {
            if (IsLeaf)
            {
                Weight = Mathf.MoveTowards(Weight, TargetWeight, Time.fixedDeltaTime * WeightSpeed);
                return Weight;
            }

            float childWeightSum = 0f;
            int maxWeightIndex = 0;
            float maxWeightVal = -1f;

            // 更新子节点权重
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                float prevW = child.Weight;
                child.UpdateNode();
                float newW = child.Weight;

                if (prevW <= 0f && newW > 0f) child.OnEnterState();
                if (prevW > 0f && newW <= 0f) child.OnExitState();

                childWeightSum += newW;
                if (newW > maxWeightVal)
                {
                    maxWeightVal = newW;
                    maxWeightIndex = i;
                }
            }

            Weight = Mathf.MoveTowards(Weight, TargetWeight, Time.fixedDeltaTime * WeightSpeed);

            // 强制归一化
            if (childWeightSum > 0.0001f)
            {
                float assignedSum = 0f;
                for (int i = 0; i < Children.Count; i++)
                {
                    if (i == maxWeightIndex) continue;
                    float w = Children[i].Weight / childWeightSum;
                    SetInputWeight(i, w);
                    assignedSum += w;
                }
                // 确保最后总和严格等于 1.0，防止浮点数误差导致向 Bind Pose 坍缩
                SetInputWeight(maxWeightIndex, 1.0f - assignedSum);
            }
            else
            {
                // 如果处于切换真空期（比如第一帧），强制把权重给到 LastActiveChild
                if (Children.Count > 0)
                {
                    int fallback = Children.IndexOf(LastActiveChild);
                    if (fallback < 0) fallback = 0;
                    for (int i = 0; i < Children.Count; i++)
                        SetInputWeight(i, i == fallback ? 1.0f : 0f);
                }
            }

            return Weight;
        }

        /// <summary>
        /// 检查节点状态
        /// </summary>
        /// <param name="parentActive">父节点是否激活</param>
        /// <returns>该节点条件是否满足</returns>
        public abstract bool CheckState(bool parentActive);
        
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="graph">PlayableGraph</param>
        /// <param name="mixIndex">当前节点所处下标</param>
        /// <returns>Playable Api节点</returns>
        public abstract Playable BuildNode(PlayableGraph graph, int mixIndex = -1);
        
        /// <summary>
        /// 进入该节点时调用
        /// </summary>
        public virtual void OnEnterState() { }
        
        /// <summary>
        /// 离开该节点后调用
        /// </summary>
        public virtual void OnExitState() { }
        
        /// <summary>
        /// 获取该下标节点当前的权重
        /// </summary>
        /// <param name="index">节点下标</param>
        /// <returns>节点权重</returns>
        protected virtual float GetInputWeight(int index) => 0f;
        
        /// <summary>
        /// 更新节点权重
        /// </summary>
        /// <param name="index">节点下标</param>
        /// <param name="w">权重值</param>
        protected virtual void SetInputWeight(int index, float w) { }

        /// <summary>
        /// 获取动画配置文件
        /// </summary>
        /// <param name="animPath">文件地址</param>
        /// <returns>配置文件 Config</returns>
        protected virtual AnimConfig GetAnimConfig(string animPath)
        {
            IResource ir = ResourceManager.instance.Load(animPath, false);
            return ir.GetAsset<AnimConfig>();
        }
    }
}