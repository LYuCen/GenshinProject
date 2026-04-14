using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimPro
{
    public class AnimTreeSlotNode<T> : AnimTreeNode<T>
    {
        private AnimTreeNode<T> _activeSubtree;    
        private AnimTreeNode<T> _fadingSubtree;    
        
        private PlayableGraph _graph;
        private AnimationMixerPlayable _slotMixer;

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
            // --- 判定是否需要过渡 ---
            // 1. 如果当前没有活跃子树，直接换。
            // 2. 如果当前插槽节点在主树中的总权重 Weight 接近 0（表示插槽当前不可见），直接换。
            // 3. 如果当前主子树虽然存在，但它内部逻辑判定没激活，直接换。
            bool isCurrentActive = _activeSubtree != null && Weight > 0.01f && _activeSubtree.Weight > 0.01f;

            if (!isCurrentActive)
            {
                // 执行【立即切换】逻辑
                ClearAllSubtrees();
                
                _activeSubtree = newSubtree;
                if (_activeSubtree != null)
                {
                    _activeSubtree.Parent = this;
                    Playable p = _activeSubtree.BuildNode(_graph, 0);
                    _graph.Connect(p, 0, _slotMixer, 0);
                    _slotMixer.SetInputWeight(0, 1f);
                    _slotMixer.SetInputWeight(1, 0f);
                    _crossfadeWeight = 1f; // 进度直接设为 1
                }
                return;
            }

            // --- 执行【平滑过渡】逻辑 ---
            if (_fadingSubtree != null)
            {
                DestroySubtree(_fadingSubtree, 1);
            }

            if (_activeSubtree != null)
            {
                _fadingSubtree = _activeSubtree;
                Playable oldPlayable = _slotMixer.GetInput(0);
                _slotMixer.DisconnectInput(0);
                _graph.Connect(oldPlayable, 0, _slotMixer, 1);
                
                // 承接之前的混合进度
                _slotMixer.SetInputWeight(1, _crossfadeWeight);
            }

            _activeSubtree = newSubtree;
            if (_activeSubtree != null)
            {
                _activeSubtree.Parent = this;
                Playable newPlayable = _activeSubtree.BuildNode(_graph, 0);
                _graph.Connect(newPlayable, 0, _slotMixer, 0);
                _slotMixer.SetInputWeight(0, 0f);
                _crossfadeWeight = 0f; // 从 0 开始淡入
            }
        }

        private void ClearAllSubtrees()
        {
            if (_fadingSubtree != null) DestroySubtree(_fadingSubtree, 1);
            if (_activeSubtree != null) DestroySubtree(_activeSubtree, 0);
            _fadingSubtree = null;
            _activeSubtree = null;
        }

        public override void Evaluate(bool parentActive = true)
        {
            bool selfActive = parentActive && CheckState(true);
            TargetWeight = selfActive ? 1f : 0f;

            _activeSubtree?.Evaluate(selfActive);
            _fadingSubtree?.Evaluate(selfActive);
        }

        public override float UpdateNode()
        {
            Weight = Mathf.MoveTowards(Weight, TargetWeight, Time.fixedDeltaTime * WeightSpeed);

            // 只有在需要过渡时（_crossfadeWeight < 1）才推进进度
            if (_crossfadeWeight < 1f)
            {
                _crossfadeWeight = Mathf.MoveTowards(_crossfadeWeight, 1f, Time.fixedDeltaTime * CrossfadeSpeed);
            }

            if (_activeSubtree != null)
            {
                float prevW = _activeSubtree.Weight;
                _activeSubtree.UpdateNode();
                if (prevW <= 0f && _activeSubtree.Weight > 0f) _activeSubtree.OnEnterState();
                _slotMixer.SetInputWeight(0, _crossfadeWeight);
            }

            if (_fadingSubtree != null)
            {
                _fadingSubtree.UpdateNode();
                float fw = 1f - _crossfadeWeight;
                _slotMixer.SetInputWeight(1, fw);

                if (fw <= 0.001f)
                {
                    DestroySubtree(_fadingSubtree, 1);
                    _fadingSubtree = null;
                }
            }

            return Weight;
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
    }
}