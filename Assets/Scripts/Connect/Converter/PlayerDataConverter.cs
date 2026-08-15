using System;
using System.Collections.Generic;
using Connect.Data;
using Connect.FlatDTO;
using Connect.MoveDTO;
using Connect.PositionDTO;
using Google.FlatBuffers;
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
                PositionDTO.SinglePositionFlat.AddLastProcessedTick(builder, s.lastProcessedTick);
                PositionDTO.SinglePositionFlat.AddPosition(builder, PositionDTO.Vec3.CreateVec3(
                    builder, (int)(s.position.x * 1000), (int)(s.position.y * 1000), (int)(s.position.z * 1000)));

                PositionDTO.SinglePositionFlat.AddYaw(builder, (ushort)(s.yaw * 100));
                PositionDTO.SinglePositionFlat.AddPitch(builder, (ushort)(s.pitch * 100));

                ushort moveState = 0;
                if (s.isGround) moveState |= 1;
                if (s.isJump) moveState |= 2;
                if (s.isSprint) moveState |= 4;
                if (s.isCrouch) moveState |= 8;
                
                PositionDTO.SinglePositionFlat.AddMoveState(builder, moveState);
                PositionDTO.SinglePositionFlat.AddLevelSpeed(builder, (ushort)(s.levelSpeed * 100));
                PositionDTO.SinglePositionFlat.AddVerticalSpeed(builder, (short)(s.verticalSpeed * 100));
                
                offsets[idx++] = PositionDTO.SinglePositionFlat.EndSinglePositionFlat(builder);
            }

            var playersVector = PositionDTO.PositionListFlat.CreatePlayersVector(builder, offsets);
            PositionDTO.PositionListFlat.StartPositionListFlat(builder);
            PositionDTO.PositionListFlat.AddPlayers(builder, playersVector);
            var root = PositionDTO.PositionListFlat.EndPositionListFlat(builder);
            builder.Finish(root.Value);
            return builder.SizedByteArray();
        }

        public static byte[] PackAnimationState(int playerId, int playerFlags, int weaponFlags, byte stance,
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
            out byte stance, out Vector3 aimPos, out float animTime, out float clientTime)
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

        public static PlayerStateData UnPackPosition(SinglePositionFlat flat)
        {
            PlayerStateData res = new PlayerStateData
            {
                playerId = flat.PlayerId,
                lastProcessedTick = flat.LastProcessedTick,
                position = new Vector3(flat.Position.Value.X / 1000f, flat.Position.Value.Y / 1000f,
                    flat.Position.Value.Z / 1000f),
                yaw = flat.Yaw / 100f,
                pitch = flat.Pitch / 100f,
                isGround = (flat.MoveState & 1) != 0,
                isJump = (flat.MoveState & 2) != 0,
                isSprint = (flat.MoveState & 4) != 0,
                isCrouch = (flat.MoveState & 8) != 0,

                levelSpeed = flat.LevelSpeed / 100f,
                verticalSpeed = flat.VerticalSpeed / 100f,
            };

            return res;
        }

        public static List<PlayerStateData> UnPackPositionList(byte[] data)
        {
            var bb = new ByteBuffer(data);
            var list = PositionListFlat.GetRootAsPositionListFlat(bb);
            var result = new List<PlayerStateData>(list.PlayersLength);
            for (int i = 0; i < list.PlayersLength; i++)
            {
                var flat = list.Players(i).Value;
                result.Add(UnPackPosition(flat));
            }

            return result;
        }
    }
}