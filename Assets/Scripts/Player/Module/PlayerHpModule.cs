using System;
using Connect;
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
        private float _waitSecond = 3f;
        private bool _isDead;
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
            if (_state.curHp <= 0)
            {
                if (!_isDead)
                {
                    _isDead = true;
                    _waitSecond = 3f;
                }

                _waitSecond -= Time.deltaTime;
                if (_waitSecond <= 0)
                {
                    _isDead = false;
                    _waitSecond = 3f;
                    Revive();
                }
            }
        }

        private void Revive()
        {
            _state.curShield = _state.maxShield;
            _state.curHp = _state.maxHp;
            
            _context.EntityStatusUI.SetShield(_state.curShield);
            _context.EntityStatusUI.SetHp(_state.curHp);

            NetworkService.Bridge.SendToServer(new Message("Revive", BitConverter.GetBytes(_state.playerId)));
        }

        public void OnFixedUpdate()
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
            if (_isDead) return; // 死亡过程不更新 hp
            
            _state.curHp = dto.CurHp;
            _state.maxHp = dto.MaxHp;
            _state.curShield = dto.CurShield;
            _state.maxShield = dto.MaxShield;

            _context.EntityStatusUI.SyncValues(dto.CurHp, dto.MaxHp, dto.CurShield, dto.MaxShield);
        }
    }
}