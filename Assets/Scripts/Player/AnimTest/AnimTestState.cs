using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using AssetBundleFramework.Core.Resource;
using mModel.ScriptableConfig;

namespace Player.AnimTest
{
    public abstract class AnimTestState<T>
    {
        protected bool IsLeaf;
        public float Weight;                // 节点自身的逻辑权重（0~1，代表过渡进度）
        protected float TargetWeight;          
        public int MixIndex;                   
        protected List<AnimTestState<T>> Children = new(); 
        protected T MyManager;                 
        public AnimTestState<T> Parent;        

        public AnimTestState(T myManager)
        {
            MyManager = myManager;
        }

        protected void AddState(AnimTestState<T> state)
        {
            state.Parent = this;
            Children.Add(state);
        }

        public virtual void Evaluate(bool parentActive = true)
        {
            // 判定自己是否应该被激活
            bool selfActive = parentActive && CheckState(parentActive);

            if (IsLeaf)
            {
                TargetWeight = selfActive ? 1f : 0f;
                return;
            }

            // Selector 逻辑：在所有子节点中选出一个满足条件的
            AnimTestState<T> selected = null;
            if (selfActive)
            {
                foreach (var child in Children)
                {
                    if (child.CheckState(true))
                    {
                        selected = child;
                        break;
                    }
                }
            }

            // 递归评估子节点
            foreach (var child in Children)
            {
                // 如果自己没被激活，所有子节点目标都是 0
                // 如果自己被激活，只有选中的子节点目标是 1
                child.Evaluate(selfActive && child == selected);
                child.TargetWeight = (selfActive && child == selected) ? 1f : 0f;
            }

            TargetWeight = selfActive ? 1f : 0f;
        }

        public virtual float UpdateNode()
        {
            if (IsLeaf)
            {
                // 叶子节点：平滑更新逻辑权重
                Weight = Mathf.MoveTowards(Weight, TargetWeight, Time.fixedDeltaTime * 5f);
                return Weight;
            }

            // 1. 递归更新子节点逻辑权重，并计算总和
            float childWeightSum = 0f;
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                float prevW = child.Weight;
                child.UpdateNode(); // 递归
                float newW = child.Weight;

                // 触发 Enter/Exit 回调（基于逻辑权重的产生/消失）
                if (prevW <= 0f && newW > 0f) child.OnEnterState();
                if (prevW > 0f && newW <= 0f) child.OnExitState();

                childWeightSum += newW;
            }

            // 2. 更新本节点自身的逻辑权重（由父 Mixer 使用）
            Weight = Mathf.MoveTowards(Weight, TargetWeight, Time.fixedDeltaTime * 5f);

            // 3. 归一化子节点权重并应用到 Mixer
            // 解决跨层级缩水的核心：
            // 即使本节点正在淡出（TargetWeight=0），只要 childWeightSum > 0，
            // 我们就通过归一化强行让 Mixer 输出 100% 的混合姿态。
            if (childWeightSum > 0.0001f)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    // 实际设置给 Mixer 的权重 = 逻辑权重 / 总权重
                    SetInputWeight(i, Children[i].Weight / childWeightSum);
                }
            }
            else
            {
                // 如果子节点全为 0，Mixer 权重才全归 0
                for (int i = 0; i < Children.Count; i++)
                    SetInputWeight(i, 0f);
            }

            return Weight;
        }

        protected abstract bool CheckState(bool parentActive);
        public abstract Playable BuildNode(PlayableGraph graph, int mixIndex = -1);
        public virtual void OnEnterState() { }
        public virtual void OnExitState() { }
        protected virtual float GetInputWeight(int index) => 0f;
        protected virtual void SetInputWeight(int index, float w) { }

        protected virtual AnimConfig GetAnimConfig(string animPath)
        {
            IResource ir = ResourceManager.instance.Load(animPath, false);
            return ir.GetAsset<AnimConfig>();
        }
    }

    public abstract class AnimMixTestState<T> : AnimTestState<T>
    {
        protected AnimationMixerPlayable _mixer;

        public AnimMixTestState(T myManager) : base(myManager)
        {
            IsLeaf = false;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            _mixer = AnimationMixerPlayable.Create(graph, Children.Count);
            for (int i = 0; i < Children.Count; i++)
            {
                Playable childPlayable = Children[i].BuildNode(graph, i);
                graph.Connect(childPlayable, 0, _mixer, i);
                _mixer.SetInputWeight(i, 0f);
            }
            return _mixer;
        }

        protected override float GetInputWeight(int index) => _mixer.IsValid() ? _mixer.GetInputWeight(index) : 0f;
        protected override void SetInputWeight(int index, float w)
        {
            if (_mixer.IsValid()) _mixer.SetInputWeight(index, w);
        }

        public override void OnExitState()
        {
            // 当父节点彻底切断本 Mixer 时，强制通知所有子节点 Exit
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