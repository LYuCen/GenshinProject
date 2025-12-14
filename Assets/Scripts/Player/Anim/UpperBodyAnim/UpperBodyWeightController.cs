using System.Collections.Generic;
using UnityEngine;

namespace Player.Anim.UpperBodyAnim
{
    public class UpperBodyWeightController
    {
        private PlayerAnimState _nowLeafState;   // 当前叶子状态
        private PlayerAnimState _nextLeafState;  // 下一个叶子状态
        private float _weight;             // 新状态的权重（0~1）
        private int _mixIndex;             // 新状态在父节点的混合索引
        private PlayerAnimState _parentState;    // 公共父状态
        private bool _enable; // 是否启用

        public void ChangeState(PlayerAnimState next)
        {
            if (next == null || !_enable) return;
            // 如果是第一次切换，直接赋值，不做过渡
            if (_nextLeafState == null)
            {
                _nowLeafState = null;
                _nextLeafState = next;
                while (next.Parent != null)
                {
                    next.Parent.SetWeight(next.MixIndex, 1);
                    next = next.Parent;
                }
                _parentState = next;
                _mixIndex = 0;
                _weight = 0f;
                return;
            }

            // 状态相同不过渡
            if (_nextLeafState == next) return;

            // 1. 记录旧状态的祖先链
            Dictionary<PlayerAnimState, bool> hasp = new();
            PlayerAnimState state = _nextLeafState;
            while (state != null)
            {
                hasp[state] = true;
                state = state.Parent;
            }

            // 2. 找到新状态和旧状态的最近公共祖先
            PlayerAnimState parentState = next.Parent;
            while (parentState != null && !hasp.ContainsKey(parentState))
            {
                parentState = parentState.Parent;
            }

            // 3. 设置旧状态链路权重
            state = _nextLeafState;
            while (state != null && state.Parent != parentState)
            {
                state.Parent.SetWeight(state.MixIndex, 1);
                state = state.Parent;
            }

            // 4. 设置新状态链路权重
            PlayerAnimState nState = next;
            while (nState != null && nState.Parent != parentState)
            {
                nState.Parent.SetWeight(nState.MixIndex, 1);
                nState = nState.Parent;
            }

            // 5. 更新状态
            _nowLeafState = _nextLeafState;
            _nextLeafState = next;
            if (_parentState != parentState)
            {
                _parentState.SetWeight(_mixIndex, 1);
            }
            _parentState = parentState;
            _mixIndex = nState.MixIndex;
            _weight = 0f; // 从0开始过渡
            _nowLeafState.OnExitState();
            _nextLeafState.OnEnterState();
        }

        public void SetEnable(bool enable)
        {
            _enable = enable;
        }

        public void Update()
        {
            if (_parentState == null || _nextLeafState == null)
            {
                if(_parentState == null) Debug.Log("父状态为空");
                if(_nextLeafState == null) Debug.Log("切换状态为空");
                return;
            }

            // 新状态权重递增
            _weight = Mathf.Min(_weight + Time.fixedDeltaTime * 7f, 1f);
            //Debug.Log(_weight);
            _parentState.SetWeight(_mixIndex, _weight);
        }
    }
}