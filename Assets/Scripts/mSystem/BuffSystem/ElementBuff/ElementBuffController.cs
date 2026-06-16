using System;
using System.Collections.Generic;
using mSystem.BuffSystem.ElementBuff.StatusEffect;
using mSystem.HurtSystem;
using mSystem.ServerTimeSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff
{
    public class ElementBuffController : MonoBehaviour
    {
        public ElementData[] Elements = new ElementData[8];
        public Dictionary<Type, IStatusEffect> _statusEffects = new();
        public int id;

        public Action<Type> RemoveBuff;
        public Action<Type> OnEffectAdded;
        public Action<Type> OnEffectRemoved;

        /// <summary>
        /// 可注入的时间源，默认使用 Unity Time.time
        /// </summary>
        public Func<float> TimeProvider = () => Time.time;

        /// <summary>
        /// 元素衰减调度
        /// </summary>
        /// <param name="i">元素下标</param>
        /// <param name="now">当前时间</param>
        private void ScheduleDecay(int i, float now)
        {
            var e = Elements[i];
            if (!e.Enable) return;
            float rate = ElementData.GetDecayRate(e.Level);
            if (rate <= 0) return;
            float val = e.GetVal(now);
            if (val <= 0) // 元素消耗结束
            {
                e.Enable = false;
                Elements[i] = e;
                return;
            }
            TimeSystem.Instance.Add(new ElementDecayEvent(this, i, now));
        }
        

        public void AddElement(HurtComponent c)
        {
            float now = TimeProvider();
            ElementData data = Elements[c.Element];
            if (!data.Enable) data = InitData(c, now);
            else data = AddData(c, data, now);
            Elements[c.Element] = data;
            ScheduleDecay(c.Element, Mathf.Max(now, data.AttachTime));
        }

        private ElementData InitData(HurtComponent c, float now)
        {
            ElementData res = new ElementData();
            res.Level = (int)c.ElementMeasure;
            res.Capacity = (int)(c.ElementMeasure * 10000 * 0.8f);
            res.AttachTime = now;
            res.Enable = true;
            return res;
        }

        private ElementData AddData(HurtComponent c, ElementData data, float now)
        {
            float addVal = c.ElementMeasure * 10000 * 0.8f;
            float currentVal = data.GetVal(now);
            ElementData res = data;

            if (addVal > data.Capacity)
            {
                // 更强的元素覆盖，刷新
                res.Capacity = (int)addVal;
                res.AttachTime = now;
            }
            else
            {
                // 补充元素量
                float newVal = Mathf.Min(currentVal + addVal, data.Capacity);
                float rate = ElementData.GetDecayRate(data.Level);
                if (rate > 0)
                {
                    // 反推 AttachTime 使 GetVal(now) == newVal
                    // newVal = Capacity - rate * (now - AttachTime)
                    // AttachTime = now - (Capacity - newVal) / rate
                    res.AttachTime = now - (data.Capacity - newVal) / rate;
                }
            }
            return res;
        }

        public void ConsumeElement(int i, float val)
        {
            float now = TimeProvider();
            var e = Elements[i];
            if (!e.Enable) return;

            float currentVal = e.GetVal(now);
            float consumeAmount = val * 10000f;
            float newVal = Mathf.Max(0, currentVal - consumeAmount);

            float rate = ElementData.GetDecayRate(e.Level);
            if (rate > 0 && newVal > 0)
            {
                // 反推新的 AttachTime
                e.AttachTime = now - (e.Capacity - newVal) / rate;
            }
            else if (newVal <= 0)
            {
                e.Enable = false;
            }

            Elements[i] = e;

            if (e.Enable) ScheduleDecay(i, now);
        }

        public float GetElement(int i)
        {
            return Elements[i].GetVal(TimeProvider()) / 10000f;
        }

        public bool IsAlive()
        {
            float now = TimeProvider();
            for (int i = 1; i < 8; i++)
            {
                if (Elements[i].Enable && Elements[i].GetVal(now) > 0) return true;
            }
            return _statusEffects.Count > 0;
        }

        public void AddEffect(IStatusEffect effect)
        {
            Type type = effect.GetType();
            if (_statusEffects.TryGetValue(type, out var old))
            {
                old.OnRemove();
                _statusEffects.Remove(type);
            }
            _statusEffects.Add(type, effect);
            OnEffectAdded?.Invoke(type);
        }

        public void RemoveEffectByType(Type type)
        {
            if (_statusEffects.TryGetValue(type, out var effect))
            {
                effect.OnRemove();
                _statusEffects.Remove(type);
                RemoveBuff?.Invoke(type);
                OnEffectRemoved?.Invoke(type);
            }
        }

        public bool FindEffect<T>() where T : IStatusEffect
        {
            return _statusEffects.ContainsKey(typeof(T));
        }

        public Sprite GetBuffSprite(Type type)
        {
            if (_statusEffects.TryGetValue(type, out var val))
                return val.GetSprite();
            Debug.Log("获取Buff图片失败");
            return null;
        }

        public float GetBuffProgress(Type type)
        {
            if (_statusEffects.TryGetValue(type, out var val))
                return val.Progress;
            return -1;
        }

        public bool HasEffectType(Type type)
        {
            return _statusEffects.ContainsKey(type);
        }
    }
}
