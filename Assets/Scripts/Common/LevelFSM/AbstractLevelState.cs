using System.Collections.Generic;
using Common.FSM;
using IUtility;
using UnityEngine;

namespace Common.LevelFSM
{
    public abstract class AbstractLevelState<T>
    {
        protected T MyManager;
        private AbstractCondition<T> _selfCondition;
        private List<AbstractCondition<T>> Conditions = new();

        public AbstractLevelState(T manager, AbstractCondition<T> selfCondition)
        {
            MyManager = manager;
            _selfCondition = selfCondition;
        }
        
        /// <summary>
        /// 进入当前状态
        /// </summary>
        public virtual void OnEnterState(){}
        
        /// <summary>
        /// 运行当前状态
        /// </summary>
        public virtual void OnUpdateState(){}
        
        /// <summary>
        /// 结束当前状态
        /// </summary>
        public virtual void OnExitState(){}
        
        // 检查条件
        public virtual bool CheckConditions(out string nextStateName)
        {
            nextStateName = string.Empty;
            foreach (var info in Conditions)
            {
                if (info != null && info.CheckCondition(out nextStateName))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool CheckSelfConditions()
        {
            return _selfCondition != null && _selfCondition.CheckCondition(out _);
        }

        // 添加条件
        public void AddCondition(AbstractCondition<T> condition, int index = -1)
        {
            if (condition == null) return;

            if (index == -1)
            {
                Conditions.Add(condition);
            }
            else if (InListUtility.IsInList(Conditions, index))
            {
                Conditions[index] = condition;
            }
            else
            {
                Debug.LogError("下标越界");
            }
        }

        // 删除条件
        public void DeleteCondition(int index)
        {
            if (InListUtility.IsInList(Conditions, index))
            {
                Conditions.RemoveAt(index);
            }
            else
            {
                Debug.LogError("下标越界");
            }
        }

        // 交换条件
        public void SwapCondition(int indexA, int indexB)
        {
            if (InListUtility.IsInList(Conditions, indexA) && InListUtility.IsInList(Conditions, indexB))
            {
                (Conditions[indexA], Conditions[indexB]) = (Conditions[indexB], Conditions[indexA]);
            }
        }

        public void SwapCondition(int indexA, string stateNameB)
        {
            int indexB = FindConditionIndex(stateNameB);
            SwapCondition(indexA, indexB);
        }

        public void SwapCondition(string stateNameA, string stateNameB)
        {
            int indexA = FindConditionIndex(stateNameA);
            int indexB = FindConditionIndex(stateNameB);
            SwapCondition(indexA, indexB);
        }

        private int FindConditionIndex(string stateName)
        {
            try
            {
                return Conditions.FindIndex(c => c.NextStateName == stateName);
            }
            catch
            {
                return -1;
            }
        }
        
        
    }
}