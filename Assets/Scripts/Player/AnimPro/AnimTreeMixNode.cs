using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimPro
{
    public abstract class AnimTreeMixNode<T> : AnimTreeNode<T>
    {
        protected AnimationMixerPlayable Mixer;

        public AnimTreeMixNode(T myMessage) : base(myMessage)
        {
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Mixer = AnimationMixerPlayable.Create(graph, Children.Count);
            for (int i = 0; i < Children.Count; i++)
            {
                Playable childPlayable = Children[i].BuildNode(graph, i);
                graph.Connect(childPlayable, 0, Mixer, i);
                Mixer.SetInputWeight(i, 0f);
            }
            return Mixer;
        }
        
        public override void Evaluate(bool parentActive = true)
        {
            // 判定自身逻辑是否满足
            bool conditionMet = CheckState(true);
            // 判定自身是否应该作为“目标”激活
            bool selfActive = parentActive && conditionMet;
            
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
            
            // 当自己的条件不满足时权重却还没降为 0
            // 就继续保持最后满足的节点，防止姿态出现错误
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
                bool isTarget = (selected == child);
                
                // 如果父级不再激活自己，子节点的目标权重必须是 0
                // 但是！子节点的 Evaluate 必须知道它是否仍是“被选中维持姿态”的那一个
                if (child != null)
                {
                    child.Evaluate(isTarget);

                    // 子节点的目标权重：只有当 selfActive 且它是被选中的那个才是 1
                    child.TargetWeight = (selfActive && isTarget) ? 1f : 0f;
                }
            }

            TargetWeight = selfActive ? 1f : 0f;
        }
        
        public override float UpdateNode()
        {
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

        protected override float GetInputWeight(int index) => Mixer.IsValid() ? Mixer.GetInputWeight(index) : 0f;
        
        protected override void SetInputWeight(int index, float w)
        {
            if (Mixer.IsValid()) Mixer.SetInputWeight(index, w);
        }

        public override void OnExitState()
        {
            // 清理状态保持引用，防止下次进入时产生姿态跳变
            LastActiveChild = null;

            // 强制清理所有子节点
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Weight > 0)
                {
                    Children[i].OnExitState();
                    Children[i].Weight = 0;
                    SetInputWeight(i, 0);
                }
            }
        }
    }
}