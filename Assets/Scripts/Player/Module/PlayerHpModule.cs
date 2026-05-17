using Connect.Data;
using Connect.FlatDTO;
using mSystem.HurtSystem;
using Player.Context;
using UnityEngine;

namespace Player.Module
{
    public class PlayerHpModule : IModule
    {
        private PlayerContext _context;
        private PlayerStateData _state;
        public PlayerHpModule(PlayerContext context)
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
            // FlatBuffers 生成的属性是 PascalCase（首字母大写）
            _state.curHp = dto.CurHp;
            _state.maxHp = dto.MaxHp;
            _state.curShield = dto.CurShield;
            _state.maxShield = dto.MaxShield;

            /* todo
            // 2. 更新 UI 表现
            if (entityStatusUI)
            {
                entityStatusUI.SyncValues(dto.CurHp, dto.MaxHp, dto.CurShield, dto.MaxShield);
            }
            if (state.curHp == 0 && gameObject.activeSelf)
            {
                Debug.Log($"[PlayerManager] {state.playerId} 玩家死亡");
                gameObject.SetActive(false);
                CreateSnapshot();
            }
            */
        }
    }
}