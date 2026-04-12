using Connect.Data;
using Connect.FlatDTO;
using Google.FlatBuffers;
using IUtility;
using UnityEngine;

namespace Connect.Converter
{
    public static class PlayerDataConverter
    {
        // 1. 从 DTO 转回本地数据 (Unpack)
        public static void FillData(byte[] buffer, PlayerStateData data)
        {
            var bb = new ByteBuffer(buffer);
            var flat = PlayerDataFlat.GetRootAsPlayerDataFlat(bb);
            
            data.playerId = flat.PlayerId;
            
            // 位拆解
            int s = flat.AllBoolState;
            data.isGround  = (s & PlayerStateBoolBit.BIT_GROUND) != 0;
            data.isJump    = (s & PlayerStateBoolBit.BIT_JUMP) != 0;
            data.isShoot   = (s & PlayerStateBoolBit.BIT_SHOOT) != 0;
            data.isReload  = (s & PlayerStateBoolBit.BIT_RELOAD) != 0;
            data.isSprint  = (s & PlayerStateBoolBit.BIT_SPRINT) != 0;
            data.isThrow   = (s & PlayerStateBoolBit.BIT_THROW) != 0;
            data.isCrouch  = (s & PlayerStateBoolBit.BIT_CROUCH) != 0;
            data.isSliding = (s & PlayerStateBoolBit.BIT_SLIDING) != 0;
            
            data.curHp = flat.CurHp;
            data.maxHp = flat.MaxHp;
            data.curShield = flat.CurShield;
            data.maxShield = flat.MaxShield;

            // 坐标还原 (注意这里的访问方式)
            if (flat.Position != null)
            {
                var pos = flat.Position.Value;
                data.position = new Vector3(pos.X, pos.Y, pos.Z);
            }

            if (flat.Rotation != null)
            {
                var rot = flat.Rotation.Value;
                data.rotation = new Quaternion(rot.X, rot.Y, rot.Z, rot.W);
            }
            
            if (flat.InputMove.HasValue)
            {
                var im = flat.InputMove.Value;
                data.inputMove = new Vector2(im.X, im.Y);
            }

            if (flat.PlayerMove.HasValue)
            {
                var pm = flat.PlayerMove.Value;
                data.playerMove = new Vector2(pm.X, pm.Y);
            }

            if (flat.AimPosition.HasValue)
            {
                var ap = flat.AimPosition.Value;
                data.aimPosition = new Vector3(ap.X, ap.Y, ap.Z);
            }
        }
        
        // 2. 从本地数据转为 DTO (Pack)
        public static byte[] ToDto(PlayerStateData data)
        {
            var builder = new FlatBufferBuilder(1024);

            // 骨骼旋转数组
            VectorOffset boneRotsOffset = default;
            if (data.bRots != null && data.bRots.Length > 0)
            {
                int boneCount = data.bRots.Length;
                ushort[] compressedRots = new ushort[boneCount * 4]; // 使用 ushort
                for (int i = 0; i < boneCount; i++)
                {
                    Quaternion q = data.bRots[i].localRotation;
                    int j = i * 4;
                    compressedRots[j + 0] = CompressionUtility.Compress(q.x);
                    compressedRots[j + 1] = CompressionUtility.Compress(q.y);
                    compressedRots[j + 2] = CompressionUtility.Compress(q.z);
                    compressedRots[j + 3] = CompressionUtility.Compress(q.w);
                }
                
                boneRotsOffset = PlayerDataFlat.CreateBoneRotsVector(builder, compressedRots);
            }
            
            // 位压缩
            int s = 0;
            if (data.isGround)  s |= PlayerStateBoolBit.BIT_GROUND;
            if (data.isJump)    s |= PlayerStateBoolBit.BIT_JUMP;
            if (data.isShoot)   s |= PlayerStateBoolBit.BIT_SHOOT;
            if (data.isReload)  s |= PlayerStateBoolBit.BIT_RELOAD;
            if (data.isSprint)  s |= PlayerStateBoolBit.BIT_SPRINT;
            if (data.isThrow)   s |= PlayerStateBoolBit.BIT_THROW;
            if (data.isCrouch)  s |= PlayerStateBoolBit.BIT_CROUCH;
            if (data.isSliding) s |= PlayerStateBoolBit.BIT_SLIDING;
            
            PlayerDataFlat.StartPlayerDataFlat(builder);
            
            PlayerDataFlat.AddPlayerId(builder, data.playerId);
            PlayerDataFlat.AddAllBoolState(builder, s);

            PlayerDataFlat.AddPosition(builder, Vec3.CreateVec3(builder, data.position.x, data.position.y, data.position.z));
            PlayerDataFlat.AddRotation(builder, Quat.CreateQuat(builder, data.rotation.x, data.rotation.y, data.rotation.z, data.rotation.w));
            PlayerDataFlat.AddAimPosition(builder, Vec3.CreateVec3(builder, data.aimPosition.x, data.aimPosition.y, data.aimPosition.z));
            PlayerDataFlat.AddInputMove(builder, Vec2.CreateVec2(builder, data.inputMove.x, data.inputMove.y));
            PlayerDataFlat.AddPlayerMove(builder, Vec2.CreateVec2(builder, data.playerMove.x, data.playerMove.y));
    
            PlayerDataFlat.AddCurHp(builder, data.curHp);
            PlayerDataFlat.AddMaxHp(builder, data.maxHp);
            PlayerDataFlat.AddCurShield(builder, data.curShield);
            PlayerDataFlat.AddMaxShield(builder, data.maxShield);

            // Hips 坐标
            if (data.bRots != null)
            {
                var hPos = data.bRots[1].localPosition;
                PlayerDataFlat.AddHipsPos(builder, Vec3.CreateVec3(builder, hPos.x, hPos.y, hPos.z));
            }

            // 填入刚才创建的向量 Offset
            if (boneRotsOffset.Value != 0) PlayerDataFlat.AddBoneRots(builder, boneRotsOffset);

            var offset = PlayerDataFlat.EndPlayerDataFlat(builder);
            builder.Finish(offset.Value);

            return builder.SizedByteArray();
        }
        
        public static Offset<PlayerDataFlat> CreateSinglePlayerOffset(FlatBufferBuilder builder, PlayerStateData data)
        {
            
            // --- 第二步：处理位压缩 (Flags) ---
            int s = 0;
            if (data.isGround)  s |= PlayerStateBoolBit.BIT_GROUND;
            if (data.isJump)    s |= PlayerStateBoolBit.BIT_JUMP;
            if (data.isShoot)   s |= PlayerStateBoolBit.BIT_SHOOT;
            if (data.isReload)  s |= PlayerStateBoolBit.BIT_RELOAD;
            if (data.isSprint)  s |= PlayerStateBoolBit.BIT_SPRINT;
            if (data.isThrow)   s |= PlayerStateBoolBit.BIT_THROW;
            if (data.isCrouch)  s |= PlayerStateBoolBit.BIT_CROUCH;
            if (data.isSliding) s |= PlayerStateBoolBit.BIT_SLIDING;

            // --- 第三步：开始构建 Table ---
            PlayerDataFlat.StartPlayerDataFlat(builder);

            // 1. 基础数据
            PlayerDataFlat.AddPlayerId(builder, data.playerId);
            PlayerDataFlat.AddAllBoolState(builder, s);
            PlayerDataFlat.AddCurHp(builder, data.curHp);
            PlayerDataFlat.AddMaxHp(builder, data.maxHp);
            PlayerDataFlat.AddCurShield(builder, data.curShield);
            PlayerDataFlat.AddMaxShield(builder, data.maxShield);

            // 2. 写入 Struct (位置、旋转、输入等)
            // 注意：Struct 是内联写入的，使用生成的 CreateVec3/CreateQuat
            PlayerDataFlat.AddPosition(builder, 
                Vec3.CreateVec3(builder, data.position.x, data.position.y, data.position.z));
            
            PlayerDataFlat.AddRotation(builder, 
                Quat.CreateQuat(builder, data.rotation.x, data.rotation.y, data.rotation.z, data.rotation.w));
            
            PlayerDataFlat.AddAimPosition(builder, 
                Vec3.CreateVec3(builder, data.aimPosition.x, data.aimPosition.y, data.aimPosition.z));
            
            PlayerDataFlat.AddInputMove(builder, 
                Vec2.CreateVec2(builder, data.inputMove.x, data.inputMove.y));
            
            PlayerDataFlat.AddPlayerMove(builder, 
                Vec2.CreateVec2(builder, data.playerMove.x, data.playerMove.y));

            // --- 第四步：完成构建并返回 ---
            return PlayerDataFlat.EndPlayerDataFlat(builder);
        }
    }
}