using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace Player.AnimPro
{
    public abstract class AnimTreeLayerMixNode<T> : AnimTreeNode<T>
    {
        protected AnimationLayerMixerPlayable LayerMixer;
        protected List<AvatarMask> ChildMasks = new();

        public AnimTreeLayerMixNode(T myManager) : base(myManager)
        {
            IsLeaf = false;
        }

        public void AddLayer(AnimTreeNode<T> node, AvatarMask mask = null)
        {
            AddNode(node);
            ChildMasks.Add(mask);
        }

        /// <summary>
        /// 只要它还没淡出完 Weight > 0，就得让子层级保持 Evaluate，防止子层级瞬间坍缩。
        /// </summary>
        public override void Evaluate(bool parentActive = true)
        {
            bool selfActive = parentActive && CheckState(true);
            TargetWeight = selfActive ? 1f : 0f;

            // 即使自己不在 Target 激活状态，但只要还在淡出进度中 Weight > 0
            // 就需要允许子层级继续维持其姿态输出 Evaluate(true)，但 TargetWeight 为 0
            bool shouldKeepEvaluatingChildren = selfActive || Weight > 0.001f;

            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                // 子层是否满足自己的业务逻辑条件
                bool childCondition = child.CheckState(true);
                
                // 如果当前层级需要维持（淡入或淡出中），则评估子节点
                // 但是只有当 selfActive 和 childCondition 同时满足时，子节点目标权重才是 1
                child.Evaluate(shouldKeepEvaluatingChildren);
                child.TargetWeight = (selfActive && childCondition) ? 1f : 0f;
            }
        }

        public override float UpdateNode()
        {
            Weight = Mathf.MoveTowards(Weight, TargetWeight, Time.fixedDeltaTime * WeightSpeed);

            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                float prevW = child.Weight;
                child.UpdateNode();
                float newW = child.Weight;

                if (prevW <= 0f && newW > 0f) child.OnEnterState();
                if (prevW > 0f && newW <= 0f) child.OnExitState();

                // LayerMixer 不做归一化，直接应用子层逻辑权重
                SetInputWeight(i, newW);
            }

            return Weight;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            LayerMixer = AnimationLayerMixerPlayable.Create(graph, Children.Count);
            for (int i = 0; i < Children.Count; i++)
            {
                Playable childPlayable = Children[i].BuildNode(graph, i);
                graph.Connect(childPlayable, 0, LayerMixer, i);
                LayerMixer.SetInputWeight(i, 0f);
                if (ChildMasks[i] != null)
                    LayerMixer.SetLayerMaskFromAvatarMask((uint)i, ChildMasks[i]);
            }
            return LayerMixer;
        }

        protected override float GetInputWeight(int index) => LayerMixer.IsValid() ? LayerMixer.GetInputWeight(index) : 0f;
        
        protected override void SetInputWeight(int index, float w)
        {
            if (LayerMixer.IsValid()) LayerMixer.SetInputWeight(index, w);
        }

        public override void OnExitState()
        {
            LastActiveChild = null; // 重置引用
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