using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimPro
{
    /// <summary>
    /// 动画插槽节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AnimTreeSlotNode<T> : AnimTreeNode<T>
    {
        protected AnimTreeNode<T> _activeSubtree;    
        protected AnimTreeNode<T> _fadingSubtree;    
        
        private PlayableGraph _graph;
        private AnimationMixerPlayable _slotMixer;
        
        private float _lastActiveSubtreeWeight = 0f;
        private float _lastFadingSubtreeWeight = 0f;

        private float _crossfadeWeight = 1f;       
        public float CrossfadeSpeed = 5f;          

        public AnimTreeSlotNode(T myMessage) : base(myMessage)
        {
            IsLeaf = false;
        }
        
        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            _graph = graph;
            MixIndex = mixIndex;
            _slotMixer = AnimationMixerPlayable.Create(graph, 2);
            return _slotMixer;
        }

        public void InstallSubtree(AnimTreeNode<T> newSubtree)
        {
            bool isCurrentActive = _activeSubtree != null && Weight > 0.01f && _activeSubtree.Weight > 0.01f;

            if (!isCurrentActive)
            {
                ClearAllSubtrees();
                _activeSubtree = newSubtree;
                if (_activeSubtree != null)
                {
                    _activeSubtree.Parent = this;
                    Playable p = _activeSubtree.BuildNode(_graph, 0);
                    _graph.Connect(p, 0, _slotMixer, 0);
                    
                    // 强制预热
                    _activeSubtree.Evaluate(true);
                    _lastActiveSubtreeWeight = _activeSubtree.UpdateNode();
                    
                    _slotMixer.SetInputWeight(0, _lastActiveSubtreeWeight);
                    _slotMixer.SetInputWeight(1, 0f);
                    _crossfadeWeight = 1f; 
                }
                return;
            }

            if (_fadingSubtree != null)
            {
                DestroySubtree(_fadingSubtree, 1);
            }

            if (_activeSubtree != null)
            {
                _fadingSubtree = _activeSubtree;
                _lastFadingSubtreeWeight = _lastActiveSubtreeWeight; // 转移权重记录
                
                Playable oldPlayable = _slotMixer.GetInput(0);
                _slotMixer.DisconnectInput(0);
                _graph.Connect(oldPlayable, 0, _slotMixer, 1);
                _slotMixer.SetInputWeight(1, _lastFadingSubtreeWeight * _crossfadeWeight);
            }

            _activeSubtree = newSubtree;
            _lastActiveSubtreeWeight = 0f; // 新子树从 0 开始
            if (_activeSubtree != null)
            {
                _activeSubtree.Parent = this;
                Playable newPlayable = _activeSubtree.BuildNode(_graph, 0);
                _graph.Connect(newPlayable, 0, _slotMixer, 0);
                _slotMixer.SetInputWeight(0, 0f);
                _crossfadeWeight = 0f; 
            }
        }

        public override float UpdateNode()
        {
            // 1. 基础权重更新：这个 Weight 是给父节点（LayerMixer）看的
            // 我们让插槽的 Weight 等于 内部子树逻辑权重的混合值
            float activeLogicW = _activeSubtree != null ? _activeSubtree.UpdateNode() : 0f;
            float fadingLogicW = _fadingSubtree != null ? _fadingSubtree.UpdateNode() : 0f;

            // 2. 过渡进度更新
            if (_crossfadeWeight < 1f)
                _crossfadeWeight = Mathf.MoveTowards(_crossfadeWeight, 1f, Time.fixedDeltaTime * CrossfadeSpeed);

            // 3. 计算插槽节点整体输出给父级的 Weight
            // 公式：新子树权重 * 进度 + 旧子树权重 * (1 - 进度)
            float combinedWeight = (activeLogicW * _crossfadeWeight) + (fadingLogicW * (1f - _crossfadeWeight));
    
            // 如果是根节点，强制为 1；否则跟随逻辑
            if (Parent == null) Weight = 1f;
            else Weight = combinedWeight;

            // 4. 【核心修复】内层 Mixer 归一化设置
            // 无论逻辑权重是多少，插槽内部的两个输入端口之和必须为 1.0
            // 这样可以保证输出给 LayerMixer 的 Pose 是“纯净”的，没有被稀释成 T-Pose
            if (_activeSubtree != null || _fadingSubtree != null)
            {
                // 只有当至少有一个子树时才进行分配
                _slotMixer.SetInputWeight(0, _crossfadeWeight);
                _slotMixer.SetInputWeight(1, 1f - _crossfadeWeight);
            }

            // 5. 状态触发逻辑 (Enter/Exit)
            // 注意：这里使用逻辑权重 activeLogicW 来判断
            if (_activeSubtree != null)
            {
                if (_lastActiveSubtreeWeight <= 0f && activeLogicW > 0f) _activeSubtree.OnEnterState();
                if (_lastActiveSubtreeWeight > 0f && activeLogicW <= 0f) _activeSubtree.OnExitState();
                _lastActiveSubtreeWeight = activeLogicW;
            }

            // 6. 清理淡出完毕的子树
            if (_fadingSubtree != null)
            {
                if (fadingLogicW <= 0f && _crossfadeWeight >= 0.99f)
                {
                    DestroySubtree(_fadingSubtree, 1);
                    _fadingSubtree = null;
                    _lastFadingSubtreeWeight = 0;
                    // 确保回到 1.0 绝对值
                    _slotMixer.SetInputWeight(0, 1.0f);
                    _slotMixer.SetInputWeight(1, 0f);
                }
            }

            return Weight;
        }

        private void ClearAllSubtrees()
        {
            if (_fadingSubtree != null) DestroySubtree(_fadingSubtree, 1);
            if (_activeSubtree != null) DestroySubtree(_activeSubtree, 0);
            _fadingSubtree = null;
            _activeSubtree = null;
            _lastActiveSubtreeWeight = 0;
            _lastFadingSubtreeWeight = 0;
        }
        

        public override void Evaluate(bool parentActive = true)
        {
            bool selfActive = parentActive && CheckState(true);
            TargetWeight = selfActive ? 1f : 0f;

            _activeSubtree?.Evaluate(selfActive);
            _fadingSubtree?.Evaluate(selfActive);
        }

        private void DestroySubtree(AnimTreeNode<T> node, int port)
        {
            node.OnExitState();
            if (_slotMixer.IsValid())
            {
                Playable p = _slotMixer.GetInput(port);
                if (p.IsValid())
                {
                    _slotMixer.DisconnectInput(port);
                    _graph.DestroyPlayable(p);
                }
            }
        }

        public override bool CheckState(bool parentActive)
        {
            // 只要主子树满足条件，或者旧子树还没淡出完，插槽就是激活的
            bool activeValid = _activeSubtree != null && _activeSubtree.CheckState(parentActive);
            bool fadingValid = _fadingSubtree != null; 
            return activeValid || fadingValid;
        }
        
        protected override void SetInputWeight(int index, float w)
        {
            if (_slotMixer.IsValid()) _slotMixer.SetInputWeight(index, w);
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