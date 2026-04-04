using System.Collections.Generic;
using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.HurtSystem;
using QFramework;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff
{
    public class ElementBuffController : MonoBehaviour, ICanRegisterEvent
    {
        // 元素附着量
        public float[] elements = new float[8];
        // 用于记录每种元素反应的内部冷却
        public float[] reactionCDs = new float[8]; 
        // 元素 UI 引用
        public GameObject[] elementUI = new GameObject[8];
        
        // 持续性效果列表 (感电、冻结、燃烧等)
        private List<IStatusEffect> _activeEffects = new List<IStatusEffect>();
        
        private float _decayTimer;

        private void Awake()
        {
            _decayTimer = Time.time + 1f;
        }

        /// <summary>
        /// 每帧(或FixedUpdate)由 System 调用，驱动所有状态更新
        /// </summary>
        public void Tick(float deltaTime)
        {
            // 1. 处理所有持续性效果
            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                _activeEffects[i].OnUpdate(deltaTime, this);
                if (_activeEffects[i].IsFinished)
                {
                    _activeEffects[i].OnRemove(this);
                    _activeEffects.RemoveAt(i);
                }
            }
            
            for (int i = 0; i < 8; i++)
            {
                if (reactionCDs[i] > 0) reactionCDs[i] = Mathf.Max(0, reactionCDs[i] - deltaTime);
            }

            // 2. 元素自然衰减 (逻辑保留，但整理到 Tick 中)
            UpdateDecay();
        }

        private void UpdateDecay()
        {
            if (Time.time < _decayTimer) return;

            for (int i = 0; i < 8; i++)
            {
                // 每秒衰减 0.16 单位元素
                elements[i] = Mathf.Max(elements[i] - 0.16f, 0);
                
                if (elements[i] <= 0)
                {
                    if (elementUI[i])
                    {
                        ElementUIObjectPool.Instance.RemoveObj(elementUI[i]);
                        elementUI[i] = null;
                    }
                }
            }
            _decayTimer = Time.time + 1f;
        }

        /// <summary>
        /// 添加元素附着
        /// </summary>
        public void AddElement(int elementType, float elementMeasure, HurtComponent component)
        {
            elements[elementType] = Mathf.Min(elementMeasure + elements[elementType], 1.8f);
            
            // 如果该元素图标未显示，则触发 UI 逻辑
            if (!elementUI[elementType])
            {
                var elementComponent = new ElementComponent
                {
                    Transform = component.Transform,
                    Controller = this,
                    Element = component.Element,
                    IsStart = true
                };
                ElementSystem.Instance.EnQueue(elementComponent);
            }
        }

        /// <summary>
        /// 添加持续反应效果 (由 Reaction 系统调用)
        /// </summary>
        public void AddEffect(IStatusEffect effect)
        {
            // 如果已经有相同类型的 Effect，可以根据需要选择叠加或刷新（这里简单处理为直接添加）
            _activeEffects.Add(effect);
        }

        /// <summary>
        /// 判断是否还需要保留 Controller
        /// </summary>
        public bool IsAlive()
        {
            if (_activeEffects.Count > 0) return true;
            for (int i = 0; i < 8; i++) if (elements[i] > 0) return true;
            return false;
        }

        private void OnDisable()
        {
            if (ElementUIObjectPool.Instance == null) return;
            
            // 清理 UI
            for (int i = 0; i < 8; i++)
            {
                if (elementUI[i])
                {
                    ElementUIObjectPool.Instance.RemoveObj(elementUI[i]);
                    elementUI[i] = null;
                }
            }
            
            // 清理效果
            foreach (var effect in _activeEffects) effect.OnRemove(this);
            _activeEffects.Clear();
        }

        public IArchitecture GetArchitecture() => GenshinArchitecture.Interface;
    }
}