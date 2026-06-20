using Connect.Data;
using Connect.FlatDTO;
using mSystem.HurtSystem;
using Player.Context;
using UnityEngine;

namespace Player.Module
{
    public class PlayerHpModule : IModule
    {
        private PlayerHpContext _context;
        private PlayerStateData _state;
        public PlayerHpModule(PlayerHpContext context)
        {
            _context = context;
            _state = context.StateData;
        }
        
        public void Init()
        {
            
        }

        public void OnUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void Reset()
        {
            
        }

        public void HurtHp(HurtComponent component)
        {
            float hurt = component.Harm;
            if(_state.curShield > 0) _context.CharacterEffectManager.PlayAuraAnimation(Color.cyan);
            else if(_state.curHp > 0) _context.CharacterEffectManager.PlayAuraAnimation(Color.red);
            
            if (hurt <= _state.curShield)
            {
                _state.curShield = Mathf.Clamp(_state.curShield - hurt, 0, _state.maxShield);
            }
            else
            {
                hurt -= _state.curShield;
                _state.curShield = 0;
                _state.curHp = Mathf.Clamp(_state.curHp - hurt, 0, _state.maxHp);
            }
        }
        
        public void SyncStatus(PlayerDataFlat dto)
        {
            _state.curHp = dto.CurHp;
            _state.maxHp = dto.MaxHp;
            _state.curShield = dto.CurShield;
            _state.maxShield = dto.MaxShield;

            _context.EntityStatusUI.SyncValues(dto.CurHp, dto.MaxHp, dto.CurShield, dto.MaxShield);
        }

        public bool IsDead() => _state.curHp <= 0;
    }
}