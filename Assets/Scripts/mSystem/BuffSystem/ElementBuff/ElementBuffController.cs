using System;
using System.Collections.Generic;
using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.HurtSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff
{
    public class ElementBuffController : MonoBehaviour
    {
        public ElementData[] Elements = new ElementData[8]; // 元素状态情况
        private Dictionary<Type, IStatusEffect> _statusEffects = new();
        // 用于暂存需要移除的 Key，避免在遍历时修改字典
        private List<Type> _removeList = new();
        public bool inQueue = false;
        public int id;

        /// <summary>
        /// FixUpdate 时间，每 0.02s 调用一次
        /// </summary>
        public void Tick()
        {
            _removeList.Clear();
            
            // 遍历字典
            foreach (var kvp in _statusEffects)
            {
                var effect = kvp.Value;
                effect.OnUpdate(this);
                
                if (effect.IsFinished)
                {
                    _removeList.Add(kvp.Key); // 记录需要移除的类型
                }
            }
            
            // 移除结束的 Buff
            foreach (var type in _removeList)
            {
                _statusEffects[type].OnRemove();
                _statusEffects.Remove(type);
            }


            // 元素自然衰减
            for (int i = 0; i < 8; i++)
            {
                if (Elements[i].Enable)
                {
                    Elements[i] = LoseData(Elements[i]);
                }
            }
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="c"></param>
        public void AddElement(HurtComponent c)
        {
            ElementData data = Elements[c.Element];
            if (!data.Enable) // 该槽位为空
            {
                data = InitData(c);
            }
            else
            {
                data = AddData(c, data);
            }

            Elements[c.Element] = data;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private ElementData InitData(HurtComponent c)
        {
            ElementData res = new ElementData();
            res.Level = (int)c.ElementMeasure;
            res.Val = (int)(c.ElementMeasure * 10000 * 0.8f);
            res.Capacity = res.Val;
            res.Enable = true;
            return res;
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="c"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private ElementData AddData(HurtComponent c, ElementData data)
        {
            int val = (int)(c.ElementMeasure * 10000 * 0.8f);
            ElementData res = data;
            if (val > data.Capacity)
            {
                res.Capacity = val;
                res.Val = val;
            }
            else res.Val = Mathf.Min(res.Val + val, res.Capacity);

            return res;
        }

        private ElementData LoseData(ElementData data)
        {
            ElementData res = data;
            if (data.Level == 1) res.Val -= 20;         // 8s
            else if (data.Level == 2) res.Val -= 32;    // 10s
            else if (data.Level == 4) res.Val -= 40;    // 16s
            
            res.Enable = res.Val > 0;
            return res;
        }

        public void AddEffect(IStatusEffect effect)
        {
            Type type = effect.GetType();
            
            // 如果字典里已经有了这个类型的 Buff，直接返回（不覆盖）
            if (_statusEffects.ContainsKey(type))
            {
                return;
            }

            _statusEffects.Add(type, effect);
        }

        /// <summary>
        /// 消耗元素
        /// </summary>
        /// <param name="i">元素下标</param>
        /// <param name="val">元素量</param>
        public void ConsumeElement(int i, float val)
        {
            Elements[i].Val = Mathf.Max(0, (int)(Elements[i].Val - val * 10000));
            Elements[i].Enable = Elements[i].Val > 0;
        }

        /// <summary>
        /// 获取元素量
        /// </summary>
        /// <param name="i">元素下标</param>
        /// <returns></returns>
        public float GetElement(int i)
        {
            return Elements[i].Val / 10000f;
        }

        public bool IsAlive()
        {
            bool res = false;
            for (int i = 0; i < 8; i++)
            {
                res |= Elements[i].Enable;
            }
            return res;
        }

        /// <summary>
        /// 获取元素反应
        /// </summary>
        /// <typeparam name="T">继承 IStatusEffect 类型</typeparam>
        /// <returns></returns>
        public bool FindEffect<T>() where T : IStatusEffect
        {
            return _statusEffects.ContainsKey(typeof(T));
        }
    }
}