using System;
using System.Collections.Generic;
using mWeapon.Component;

namespace mWeapon
{
    /// <summary>
    /// 供外界读取的武器状态数据
    /// </summary>
    public class WeaponRuntime : IReadOnlyWeaponRuntime
    {
        private readonly HashSet<WeaponStateTags> _states = new();
        public MagazineComponent Component { get; set; }
        public WeaponStance Stance { get; private set; }
        public event Action OnAmmoChanged;
        public event Action OnStateChanged;
        public event Action OnReset;

        public bool HasState(WeaponStateTags tag)
        {
            return _states.Contains(tag);
        }

        public void AddState(WeaponStateTags tag)
        {
            _states.Add(tag);
        }

        public void RemoveState(WeaponStateTags tag)
        {
            _states.Remove(tag);
        }
        
        public void SetStance(WeaponStance stance)
        {
            if (Stance == stance) return;
            Stance = stance;

            OnStateChanged?.Invoke();
        }

        public void NotifyAmmoChanged() => OnAmmoChanged?.Invoke();

        public void Reset()
        {
            _states.Clear(); // 清除字典状态
            Stance = WeaponStance.Normal; // 清除据枪状态
            OnReset?.Invoke();
        }
    }
}