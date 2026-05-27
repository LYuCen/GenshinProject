using AssetBundleFramework.Core.Resource;
using Connect;
using Connect.FlatDTO;
using GameFSM.LevelFSM.PlayerFSM;
using mModel.ScriptableConfig;
using mWeapon;
using mWeapon.ComponentData;
using mWeapon.Presenter;
using Player.AnimTree.UpperBodyNodes;
using Player.Module;
using UnityEngine;

namespace Player.ManagerLogic
{
    public class RemotePlayerLogic : IPlayerLogic
    {
        private PlayerManager _m;
        
        // --- 预测相关变量 ---
        private Vector3 _lastServerPos;      // 上一帧服务器发来的位置
        private Vector3 _currentVelocity;    // 当前预测的速度
        private Quaternion _targetRot;       // 目标旋转
        private float _lastPacketTime;       // 上次收到包的时间
        private bool _isFirstPacket = true;  // 是否是第一个包

        private const float MaxSnapDistance = 2.0f;
        private PlayerFsm _playerFsm;
        private WeaponPresenter weapon;

        public void Init(PlayerManager manager)
        {
            Debug.Log("创建其他玩家");
            _m = manager;

            // 1. 上下文
            manager.CombatContext.InputContext = null;
            manager.CombatContext.StateData = manager.state;
            manager.CombatContext.IsLocal = false;
            manager.IKContext.ListenEvents = false;
            manager.IKContext.IsLocal = false;

            // 2. 模块
            manager.CombatModule = new PlayerCombatModule(manager.CombatContext);
            manager.MoveModule = new PlayerMoveModule(manager.MoveContext);
            manager.IkModule = new PlayerIkModule(manager.IKContext);

            _lastPacketTime = Time.time;

            manager.CombatModule.Init();
            manager.MoveModule.Init();
            manager.IkModule.Init();

            // 3. 武器（动画树依赖 MoveModule，IK 依赖武器 Transform）
            InitTransformPosition("FnIws");
            manager.IkModule.SetMuzzle(weapon.muzzle);
            manager.IkModule.SetHandIk(weapon.hand);

            // 4. FSM（依赖 MoveModule）
            _playerFsm = new PlayerFsm(manager);

        }

        private void InitTransformPosition(string name)
        {
            var ir2 = ResourceManager.instance.Load
                ($"Assets/AssetBundle/ScriptableObject/Weapon/{name}/{name}.asset", false);
            var data = ir2.GetAsset<WeaponData>();
            IResource ir = ResourceManager.instance.Load($"Assets/AssetBundle/Model/Weapon/Remote/{data.weaponName}.prefab", false);
            GameObject prefab = Object.Instantiate(ir.GetAsset<GameObject>(), _m.gunCube, false);
            weapon = prefab.GetComponent<WeaponPresenter>();
            var ir3 = ResourceManager.instance.Load(
                $"Assets/AssetBundle/ScriptableObject/Weapon/{data.name}/{name}_Trans.asset",false);
            var trans = ir3.GetAsset<TransformComponentData>();
            prefab.transform.localPosition = trans.pos;
            prefab.transform.localRotation = Quaternion.Euler(trans.rot.x, trans.rot.y, trans.rot.z);

            var ir4 = ResourceManager.instance.Load(
                $"Assets/AssetBundle/ScriptableObject/Weapon/{name}/{name}_Anim.asset", false);
            var anim = ir4.GetAsset<AnimComponentData>();
            Debug.Log($"[Bot] Graph valid before switch:{_m.AnimController.Graph.IsValid()}");
            _m.AnimController.SwitchWeaponGraph(anim.animGraphData, data.weaponType);
            TagClipPair tagClipPair = new TagClipPair();
            for (int i = 0; i < anim.animMappings.Count; i++)
            {
                if (anim.animMappings[i].tag == "Reload")
                {
                    tagClipPair = anim.animMappings[i];
                    break;
                }
            }
            _m.AnimController.InstallSlotAction("UpperSlot", manager =>
            {
                var node = new ReloadTreeLeafNode(manager);
                node.AnimSpeed = tagClipPair.animSpeed;
                node.Tag = tagClipPair.tag;
                node.UseTagOnly = false;
                node.DefaultClip = tagClipPair.clip;
                node.CurrentData = tagClipPair.clipAsset;
                return node;
            });
        }

        // 修改参数为 PlayerDataFlat
        public void SyncFromServer(PlayerDataFlat dto)
        {
            // 1. 手动还原数据到 _m.state (POCO对象)
            // 你可以把这段逻辑写进 PlayerDataConverter 的重载里，这里为了直观直接写出
            UnpackFlatToState(dto);

            // 2. 提取平滑同步所需的变量
            if (dto.Position != null)
            {
                var pos = dto.Position.Value;
                Vector3 newPos = new Vector3(pos.X, pos.Y, pos.Z);

                if (dto.Rotation != null)
                {
                    var rot = dto.Rotation.Value;
                    _targetRot = new Quaternion(rot.X, rot.Y, rot.Z, rot.W);
                }

                if (_isFirstPacket)
                {
                    _m.transform.position = newPos;
                    _m.transform.rotation = _targetRot;
                    _lastServerPos = newPos;
                    _isFirstPacket = false;
                    return;
                }

                // 3. 计算预测速度
                float timeSinceLastPacket = Time.time - _lastPacketTime;
                timeSinceLastPacket = Mathf.Max(timeSinceLastPacket, 0.01f);

                _currentVelocity = (newPos - _lastServerPos) / timeSinceLastPacket;
            
                _lastServerPos = newPos;
                _lastPacketTime = Time.time;
            
                // 4. 用服务器同步的 aim 位置驱动 IK 目标
                if (dto.AimPosition != null)
                {
                    var aimPos = dto.AimPosition.Value;
                    _m.IkModule.SetAimTargetPosition(new Vector3(aimPos.X, aimPos.Y, aimPos.Z));
                }

                // 5. 更新 IK 状态
                _m.aimIK.enabled = _m.state.isShoot;

                // 6. 距离过大补偿
                if (Vector3.Distance(_m.transform.position, newPos) > MaxSnapDistance)
                {
                    _m.transform.position = newPos;
                }
            }
        }

        // 辅助方法：将二进制对象解包到本地 State 对象
        private void UnpackFlatToState(PlayerDataFlat dto)
        {
            // 位拆解
            int s = dto.AllBoolState;
            _m.state.isGround  = (s & PlayerStateBoolBit.BIT_GROUND) != 0;
            _m.state.isJump    = (s & PlayerStateBoolBit.BIT_JUMP) != 0;
            _m.state.isShoot   = (s & PlayerStateBoolBit.BIT_SHOOT) != 0;
            _m.state.isReload  = (s & PlayerStateBoolBit.BIT_RELOAD) != 0;
            _m.state.isSprint  = (s & PlayerStateBoolBit.BIT_SPRINT) != 0;
            _m.state.isThrow   = (s & PlayerStateBoolBit.BIT_THROW) != 0;
            _m.state.isCrouch  = (s & PlayerStateBoolBit.BIT_CROUCH) != 0;
            _m.state.isSliding = (s & PlayerStateBoolBit.BIT_SLIDING) != 0;

            // 数值同步
            _m.state.curHp = dto.CurHp;
            _m.state.maxHp = dto.MaxHp;
            _m.state.curShield = dto.CurShield;
            _m.state.maxShield = dto.MaxShield;

            // 移动同步 (Animation 参数使用)
            if (dto.InputMove != null)
            {
                var input = dto.InputMove.Value;
                _m.state.inputMove = new Vector2(input.X, input.Y);
            }

            if (dto.PlayerMove != null)
            {
                var pMove = dto.PlayerMove.Value;
                _m.state.playerMove = new Vector2(pMove.X, pMove.Y);
            }
        }

        public void OnUpdate()
        {
            _playerFsm.Update();
            _m.AnimController.Update();
            
            // 这里的预测插值算法逻辑保持不变...
            Vector3 predictionMove = _currentVelocity * Time.deltaTime;
            Vector3 correction = (_lastServerPos - _m.transform.position) * 0.15f;

            _m.transform.position += predictionMove + correction;
            _m.transform.rotation = Quaternion.Slerp(_m.transform.rotation, _targetRot, Time.deltaTime * 15f);
            
            _m.IkModule?.OnUpdate();
            
            if (Time.time - _lastPacketTime > 0.2f)
            {
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, Time.deltaTime * 5f);
            }
            
        }

        public void OnFixedUpdate() { }
        public void OnLateUpdate() { }
    }
}