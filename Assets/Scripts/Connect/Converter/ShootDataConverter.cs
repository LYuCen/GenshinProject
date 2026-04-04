using Connect.Data;
using Connect.DTO;
using UnityEngine;

namespace Connect.Converter
{
    public static class ShootDataConverter
    {
        // 1. DTO → Data（解包）
        public static void FillData(ShootDto dto, ShootData data)
        {
            data.shooterId = dto.shooterId;

            data.position = new Vector3(dto.px, dto.py, dto.pz);
            data.forward  = new Vector3(dto.fx, dto.fy, dto.fz);

            data.speed = dto.speed;
            data.element = dto.element;
            data.serverTick = dto.serverTick;
            data.baseHit = dto.baseHit;
            data.headHit = dto.headHit;
            data.elementMeasure = dto.elementMeasure;
            data.moveTime = dto.moveTime;
        }

        // 2. Data → DTO（打包）
        public static ShootDto ToDto(ShootData data)
        {
            ShootDto dto = new ShootDto();

            dto.shooterId = data.shooterId;

            // position
            dto.px = data.position.x;
            dto.py = data.position.y;
            dto.pz = data.position.z;

            // forward
            dto.fx = data.forward.x;
            dto.fy = data.forward.y;
            dto.fz = data.forward.z;

            dto.speed = data.speed;
            dto.element = data.element;
            dto.serverTick = data.serverTick;
            dto.baseHit = data.baseHit;
            dto.headHit = data.headHit;
            dto.elementMeasure = data.elementMeasure;
            dto.moveTime = data.moveTime;

            return dto;
        }
    }
}