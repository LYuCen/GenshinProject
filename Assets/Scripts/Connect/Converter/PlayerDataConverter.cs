using Connect.Data;
using Connect.DTO;
using UnityEngine;

namespace Connect.Converter
{
    public static class PlayerDataConverter
    {
        // 定义位偏移常量
        private const int BIT_GROUND  = 1 << 0;
        private const int BIT_JUMP    = 1 << 1;
        private const int BIT_SHOOT   = 1 << 2;
        private const int BIT_RELOAD  = 1 << 3;
        private const int BIT_SPRINT  = 1 << 4;
        private const int BIT_THROW   = 1 << 5;
        private const int BIT_CROUCH  = 1 << 6;
        private const int BIT_SLIDING = 1 << 7;

        // 1. 从 DTO 转回本地数据 (Unpack)
        public static void FillData(PlayerDataDto dto, PlayerStateData data)
        {
            data.playerId = dto.playerId;
            
            // 位拆解
            int s = dto.allBoolState;
            data.isGround  = (s & BIT_GROUND) != 0;
            data.isJump    = (s & BIT_JUMP) != 0;
            data.isShoot   = (s & BIT_SHOOT) != 0;
            data.isReload  = (s & BIT_RELOAD) != 0;
            data.isSprint  = (s & BIT_SPRINT) != 0;
            data.isThrow   = (s & BIT_THROW) != 0;
            data.isCrouch  = (s & BIT_CROUCH) != 0;
            data.isSliding = (s & BIT_SLIDING) != 0;

            // 坐标还原
            data.position = new Vector3(dto.px, dto.py, dto.pz);
            data.rotation = new Quaternion(dto.rx, dto.ry, dto.rz, dto.rw);
            data.aimPosition = new Vector3(dto.ax, dto.ay, dto.az);
            data.inputMove = new Vector2(dto.ix, dto.iy);
            data.playerMove = new Vector2(dto.mx, dto.my);
            data.curHp = dto.curHp;
            data.maxHp = dto.maxHp;
            data.curShield = dto.curShield;
            data.maxShield = dto.maxShield;
        }
        
        // 2. 从本地数据转为 DTO (Pack)
        public static PlayerDataDto ToDto(PlayerStateData data)
        {
            PlayerDataDto dto = new PlayerDataDto();
            dto.playerId = data.playerId;

            // 位压缩
            int s = 0;
            if (data.isGround)  s |= BIT_GROUND;
            if (data.isJump)    s |= BIT_JUMP;
            if (data.isShoot)   s |= BIT_SHOOT;
            if (data.isReload)  s |= BIT_RELOAD;
            if (data.isSprint)  s |= BIT_SPRINT;
            if (data.isThrow)   s |= BIT_THROW;
            if (data.isCrouch)  s |= BIT_CROUCH;
            if (data.isSliding) s |= BIT_SLIDING;
            dto.allBoolState = s;

            // 坐标赋值 (直接赋值 float，无 GC)
            dto.px = data.position.x; dto.py = data.position.y; dto.pz = data.position.z;
            dto.rx = data.rotation.x; dto.ry = data.rotation.y; dto.rz = data.rotation.z; dto.rw = data.rotation.w;
            dto.ax = data.aimPosition.x; dto.ay = data.aimPosition.y; dto.az = data.aimPosition.z;
            dto.ix = data.inputMove.x; dto.iy = data.inputMove.y;
            dto.mx = data.playerMove.x; dto.my = data.playerMove.y;
            dto.curHp = data.curHp;
            dto.maxHp = data.maxHp;
            dto.curShield = data.curShield;
            dto.maxShield = data.maxShield;

            return dto;
        }
    }
}