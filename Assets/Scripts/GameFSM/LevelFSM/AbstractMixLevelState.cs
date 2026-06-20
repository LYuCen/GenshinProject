using System.Collections.Generic;
using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM
{
    public class AbstractMixLevelState<T> : AbstractLevelState<T>
    {
        // 状态字典
        private Dictionary<string, AbstractLevelState<T>> _mStates;

        public AbstractMixLevelState(T manager, AbstractCondition<T> selfCondition) : base(manager, selfCondition)
        {
            IsBuilt = false;
            Init();
        }

        public Dictionary<string, AbstractLevelState<T>> States
        {
            get
            {
                if (_mStates == null)
                {
                    _mStates = new Dictionary<string, AbstractLevelState<T>>();
                }

                return _mStates;
            }
            set
            {
                _mStates = value;
            }
        }
        
        // 当前状态
        private AbstractLevelState<T> _mCurrentState;
        public AbstractLevelState<T> CurrentState
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
        private AbstractLevelState<T> _mDefaultState;
        public bool IsBuilt { get; protected set; } // 是否创建完成

        
        
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init(){BuildState();}

        /// <summary>
        /// 创建状态节点
        /// </summary>
        protected virtual void BuildState(){}

        public override void Enter()
        {
            OnEnterState();
            
            if (_mCurrentState == null)
            {
                // 优先使用默认状态
                if (_mDefaultState != null)
                {
                    _mCurrentState = _mDefaultState;
                }
                else
                {
                    // 否则找第一个满足条件的状态
                    foreach (var state in States.Values)
                    {
                        if (state.CheckSelfConditions())
                        {
                            _mCurrentState = state;
                            break;
                        }
                    }
                }
            }
            
            // 传递 OnEnter
            _mCurrentState?.Enter();
        }

        public override void Exit()
        {
            OnExitState();
            _mCurrentState?.OnExitState();
        }

        public override void Update()
        {
            OnUpdateState();
            if (_mCurrentState == null) // 如果初始状态为空，那就搜索第一个满足的状态
            {
                foreach (var state in _mStates)
                {
                    if (state.Value.CheckSelfConditions())
                    {
                        _mCurrentState = state.Value;
                        _mCurrentState.Enter();
                        break;
                    }
                }
            }
            if (_mCurrentState == null || _mStates == null || !IsBuilt)
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
            else _mCurrentState.Update();
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
        
        private void ChangeState(AbstractLevelState<T> nextState)
        {
            if (nextState == _mCurrentState) return; // 避免同状态重复进入
            
            // 结束上一个状态，如果上个状态不为空
            _mCurrentState?.Exit();

            if (nextState != null) // 如果下一个状态不为空
            {
                _mCurrentState = nextState;
                // 开始状态
                _mCurrentState.Enter();
            }
        }
    }
}