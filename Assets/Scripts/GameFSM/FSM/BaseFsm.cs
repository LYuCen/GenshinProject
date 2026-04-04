using System.Collections.Generic;
using UnityEngine;

namespace GameFSM.FSM
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public class BaseFsm<T>
    {
        // 状态字典
        private Dictionary<string, AbstractState<T>> _mStates;
        public Dictionary<string, AbstractState<T>> States
        {
            get
            {
                if (_mStates == null)
                {
                    _mStates = new Dictionary<string, AbstractState<T>>();
                }

                return _mStates;
            }
            set
            {
                _mStates = value;
            }
        }

        // 当前状态
        private AbstractState<T> _mCurrentState;
        public AbstractState<T> CurrentState
        {
            get
            {
                return _mCurrentState;
            }
            private set
            {
                _mCurrentState = value;
            }
        }
        
        // 默认状态
        private AbstractState<T> _mDefaultState;
        
        // 状态构建器
        private AbstractStateBuilder<T> _mStateBuilder;

        public BaseFsm(AbstractStateBuilder<T> stateBuilder)
        {
            _mStateBuilder = stateBuilder;
            _mStates = stateBuilder.CreateStates();
        }

        public void Update()
        {
            if (_mCurrentState == null || _mStates == null || !_mStateBuilder.IsBuilt)
            {
                return;
            }
            
            // 检查是否满足转移条件满足就转移，否则继续运行 Update
            if (_mCurrentState.CheckConditions(out string nextStateName))
            {
                if (States.TryGetValue(nextStateName, out var state))
                {
                    ChangeState(state);
                }
            }
            else _mCurrentState.OnUpdateState();
        }

        /// <summary>
        /// 设置默认状态
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="startEnter">如果为 true 立刻执行 OnEnter 方法</param>
        public void SetDefaultState(string stateName, bool startEnter = true)
        {
            if (States.ContainsKey(stateName))
            {
                _mDefaultState = States[stateName];
                _mCurrentState = _mDefaultState;
                if (startEnter)
                {
                    _mCurrentState.OnEnterState();
                }
            }
            else
            {
                Debug.LogError($"未发现该键{stateName}");
            }
        }

        private void ChangeState(AbstractState<T> nextState)
        {
            // 结束上一个状态，如果上个状态不为空
            _mCurrentState?.OnExitState();

            if (nextState != null) // 如果下一个状态不为空
            {
                _mCurrentState = nextState;
                // 开始状态
                _mCurrentState.OnEnterState();
            }
        }
    }
}