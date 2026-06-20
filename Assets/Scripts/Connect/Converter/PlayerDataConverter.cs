using System;
using System.Collections.Generic;
using Connect.BoneDTO;
using Connect.Data;
using Connect.FlatDTO;
using Connect.MoveDTO;
using Google.FlatBuffers;
using IUtility;
using mEnum.Query;
using UnityEngine;
using Quat = Connect.FlatDTO.Quat;
using Vec3 = Connect.FlatDTO.Vec3;

namespace Connect.Converter
{
    public static class PlayerDataConverter
    {
        public static Offset<PlayerDataFlat> CreateSinglePlayerOffset(FlatBufferBuilder builder, PlayerStateData data)
        {
            PlayerDataFlat.StartPlayerDataFlat(builder);
            
            PlayerDataFlat.AddPlayerId(builder, data.playerId);
            PlayerDataFlat.AddCurHp(builder, data.curHp);
            PlayerDataFlat.AddMaxHp(builder, data.maxHp);
            PlayerDataFlat.AddCurShield(builder, data.curShield);
            PlayerDataFlat.AddMaxShield(builder, data.maxShield);
            
            PlayerDataFlat.AddPosition(builder, 
                Vec3.CreateVec3(builder, data.position.x, data.position.y, data.position.z));
            
            PlayerDataFlat.AddRotation(builder, 
                Quat.CreateQuat(builder, data.rotation.x, data.rotation.y, data.rotation.z, data.rotation.w));
            
            return PlayerDataFlat.EndPlayerDataFlat(builder);
        }

        /// <summary>
        /// C2S_Move 仅位置旋转
        /// </summary>
        public static byte[] PackMove(PlayerStateData data)
        {
            var builder = new FlatBufferBuilder(128);

            MoveDataFlat.StartMoveDataFlat(builder);
            MoveDataFlat.AddPlayerId(builder, data.playerId);
            MoveDataFlat.AddPosition(builder,
                Connect.MoveDTO.Vec3.CreateVec3(builder, data.position.x, data.position.y, data.position.z));
            MoveDataFlat.AddRotation(builder,
                Connect.MoveDTO.Quat.CreateQuat(builder, data.rotation.x, data.rotation.y, data.rotation.z,
                    data.rotation.w));
            MoveDataFlat.AddInputX(builder, data.inputMove.x);
            MoveDataFlat.AddInputY(builder, data.inputMove.y);
            MoveDataFlat.AddMoveX(builder, data.playerMove.x);
            MoveDataFlat.AddMoveY(builder, data.playerMove.y);

            var offset = MoveDataFlat.EndMoveDataFlat(builder);
            builder.Finish(offset.Value);
            return builder.SizedByteArray();
        }

        /// <summary>
        /// 解包 C2S_Move
        /// </summary>
        public static void UnpackMove(byte[] buf, PlayerStateData data)
        {
            var bb = new ByteBuffer(buf);
            var flat = MoveDataFlat.GetRootAsMoveDataFlat(bb);

            data.playerId = flat.PlayerId;

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

            data.inputMove = new Vector2(flat.InputX, flat.InputY);
            data.playerMove = new Vector2(flat.MoveX, flat.MoveY);
        }

        /// <summary>
        /// 骨骼旋转 瞄准点 重心位置
        /// </summary>
        public static byte[] PackBones(PlayerStateData data)
        {
            var builder = new FlatBufferBuilder(512);

            ushort[] compressedRots = null;
            if (data.bRots != null && data.bRots.Length > 0)
            {
                compressedRots = new ushort[data.bRots.Length * 4];
                for (int i = 0; i < data.bRots.Length; i++)
                {
                    Quaternion q = data.bRots[i].localRotation;
                    compressedRots[i * 4 + 0] = CompressionUtility.Compress(q.x);
                    compressedRots[i * 4 + 1] = CompressionUtility.Compress(q.y);
                    compressedRots[i * 4 + 2] = CompressionUtility.Compress(q.z);
                    compressedRots[i * 4 + 3] = CompressionUtility.Compress(q.w);
                }
            }

            var boneVector = compressedRots != null
                ? BoneDataFlat.CreateBoneRotsVector(builder, compressedRots)
                : default;

            Vector3 hips = (data.bRots != null && data.bRots.Length > 1)
                ? data.bRots[1].localPosition : Vector3.zero;
            
            BoneDataFlat.StartBoneDataFlat(builder);
            BoneDataFlat.AddPlayerId(builder, data.playerId);
            BoneDataFlat.AddAimX(builder, data.aimPosition.x);
            BoneDataFlat.AddAimY(builder, data.aimPosition.y);
            BoneDataFlat.AddAimZ(builder, data.aimPosition.z);
            
            BoneDataFlat.AddHipsX(builder, hips.x);
            BoneDataFlat.AddHipsY(builder, hips.y);
            BoneDataFlat.AddHipsZ(builder, hips.z);

            if (boneVector.Value != 0)
            {
                BoneDataFlat.AddBoneRots(builder, boneVector);
            }

            BoneDataFlat.AddClientTime(builder, data.clientTime);
            var offset = BoneDataFlat.EndBoneDataFlat(builder);
            builder.Finish(offset.Value);
            return builder.SizedByteArray();
        }

        public static void UnpackBone(byte[] buf, PlayerStateData data)
        {
            var bb = new ByteBuffer(buf);
            var flat = BoneDataFlat.GetRootAsBoneDataFlat(bb);

            data.playerId = flat.PlayerId;
            data.aimPosition = new Vector3(flat.AimX, flat.AimY, flat.AimZ);
            data.clientTime = flat.ClientTime;
            
            //return;
            int boneCount = flat.BoneRotsLength / 4;
            if (data.bRots != null && boneCount > 0)
            {
                for (int i = 0; i < Mathf.Min(boneCount, data.bRots.Length); i++)
                {
                    data.bRots[i].localRotation = new
                        Quaternion(
                            CompressionUtility.Decompress(flat.BoneRots(i * 4)),
                            CompressionUtility.Decompress(flat.BoneRots(i * 4 + 1)),
                            CompressionUtility.Decompress(flat.BoneRots(i * 4 + 2)), 
                            CompressionUtility.Decompress(flat.BoneRots(i * 4 + 3)));
                }

                // 重心骨骼
                data.bRots[1].localPosition = new Vector3(flat.HipsX, flat.HipsY, flat.HipsZ);
            }
            
        }
        
        /// <summary>
        /// 血量
        /// </summary>
        public static byte[] PackHp(int playerId, float curHp, float curShield, float maxHp, float maxShield)
        {
            byte[] buf = new byte[20];
            int p = 0;

            Buffer.BlockCopy(BitConverter.GetBytes(playerId), 0, buf, p, 4); p += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(curHp), 0, buf, p, 4); p += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(curShield), 0, buf, p, 4); p += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(maxHp), 0, buf, p, 4); p += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(maxShield), 0, buf, p, 4);
            return buf;
        }

        public static void UnPackHp(byte[] buf, out int playerId, out float curHp, out float curShield, out float maxHp, out float maxShield)
        {
            int p = 0;
            playerId  = BitConverter.ToInt32(buf, p);  p += 4;
            curHp     = BitConverter.ToSingle(buf, p); p += 4;
            curShield = BitConverter.ToSingle(buf, p); p += 4;
            maxHp     = BitConverter.ToSingle(buf, p); p += 4;
            maxShield = BitConverter.ToSingle(buf, p);      
        }
        
        /// <summary>
        /// 打包单个玩家完整数据 (含HP)
        /// </summary>
        public static byte[] PackSinglePlayer(PlayerStateData data)
        {
            var builder = new FlatBufferBuilder(256);
            var offset = CreateSinglePlayerOffset(builder, data);
            builder.Finish(offset.Value);
            return builder.SizedByteArray();
        }

        /// <summary>
        /// 打包全量玩家完整数据 (含HP)
        /// </summary>
        public static byte[] PackAllPlayers(Dictionary<int, Server.ServerPlayerObject> players)
        {
            var builder = new FlatBufferBuilder(players.Count * 256 + 64);
            var offsets = new Offset<PlayerDataFlat>[players.Count];
            int idx = 0;
            foreach (var kv in players)
            {
                offsets[idx++] = CreateSinglePlayerOffset(builder, kv.Value.State);
            }
            var playersVector = PlayerList.CreatePlayersVector(builder, offsets);
            PlayerList.StartPlayerList(builder);
            PlayerList.AddPlayers(builder, playersVector);
            var root = PlayerList.EndPlayerList(builder);
            builder.Finish(root.Value);
            return builder.SizedByteArray();
        }

        public static byte[] PackPositions(Dictionary<int, Server.ServerPlayerObject> players)
        {  
            var builder = new FlatBufferBuilder(players.Count * 36 + 64);
     
            var offsets = new Offset<PositionDTO.SinglePositionFlat>[players.Count];
            int idx = 0;
            foreach (var kv in players)
            {
                var s = kv.Value.State;
                PositionDTO.SinglePositionFlat.StartSinglePositionFlat(builder);

                PositionDTO.SinglePositionFlat.AddPlayerId(builder, s.playerId);

                PositionDTO.SinglePositionFlat.AddPosition(builder, 
                    PositionDTO.Vec3.CreateVec3(builder,    
                        s.position.x, s.position.y, s.position.z));

                PositionDTO.SinglePositionFlat.AddRotation(builder, 
                    PositionDTO.Quat.CreateQuat(builder, 
                        s.rotation.x, s.rotation.y, s.rotation.z, s.rotation.w));
                
                PositionDTO.SinglePositionFlat.AddInputX(builder, (int)s.inputMove.x);
                PositionDTO.SinglePositionFlat.AddInputY(builder, (int)s.inputMove.y);
                PositionDTO.SinglePositionFlat.AddAim(builder,
                    PositionDTO.Vec3.CreateVec3(builder, s.aimPosition.x, s.aimPosition.y, s.aimPosition.z));
                
                offsets[idx++] = PositionDTO.SinglePositionFlat.EndSinglePositionFlat(builder);
            }

            var playersVector = PositionDTO.PositionListFlat.CreatePlayersVector(builder, offsets);
            PositionDTO.PositionListFlat.StartPositionListFlat(builder);
            PositionDTO.PositionListFlat.AddPlayers(builder, playersVector);
            var root = PositionDTO.PositionListFlat.EndPositionListFlat(builder);
            builder.Finish(root.Value);
            return builder.SizedByteArray();
        }

        public static byte[] PackAnimationState(int playerId, int playerFlags, int weaponFlags, sbyte stance,
            Vector3 aimPos, float animTime, float clientTime)
        {
            var builder = new FlatBufferBuilder(64);
            AnimDTO.AnimDataFlat.StartAnimDataFlat(builder);
            AnimDTO.AnimDataFlat.AddPlayerId(builder, playerId);
            AnimDTO.AnimDataFlat.AddPlayerBoolState(builder, playerFlags);
            AnimDTO.AnimDataFlat.AddWeaponBoolState(builder, weaponFlags);
            AnimDTO.AnimDataFlat.AddStance(builder, stance);
            AnimDTO.AnimDataFlat.AddAimPos(builder,
                AnimDTO.Vec3.CreateVec3(builder, aimPos.x, aimPos.y, aimPos.z));
            AnimDTO.AnimDataFlat.AddAnimTime(builder, animTime);

            AnimDTO.AnimDataFlat.AddClientTime(builder, clientTime);
            var offset = AnimDTO.AnimDataFlat.EndAnimDataFlat(builder);
            builder.Finish(offset.Value);
            return builder.SizedByteArray();
        }

        public static void UnpackAnimationState(byte[] data, out int playerId, out int playerFlags, out int weaponFlags,
            out sbyte stance, out Vector3 aimPos, out float animTime, out float clientTime)
        {
            var bb = new ByteBuffer(data);
            var flat = AnimDTO.AnimDataFlat.GetRootAsAnimDataFlat(bb);
            playerId = flat.PlayerId;
            playerFlags = flat.PlayerBoolState;
            weaponFlags = flat.WeaponBoolState;
            stance = flat.Stance;
            aimPos = flat.AimPos != null
                ? new Vector3(flat.AimPos.Value.X, flat.AimPos.Value.Y, flat.AimPos.Value.Z)
                : Vector3.zero;
            animTime = flat.AnimTime;
            clientTime = flat.ClientTime;
        }
        
    }
}