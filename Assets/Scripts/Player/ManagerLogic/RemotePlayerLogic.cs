using AssetBundleFramework.Core.Resource;
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
        
        private PlayerFsm _playerFsm;
        private WeaponPresenter _weapon;

        public void Init(PlayerManager manager)
        {
            Debug.Log("创建其他玩家");
            _m = manager;
            
            manager.CombatContext.InputContext = null;
            manager.CombatContext.StateData = manager.state;
            manager.CombatContext.IsLocal = false;
            manager.IKContext.ListenEvents = false;
            manager.IKContext.IsLocal = false;
            manager.MoveContext.IsLocal = false;
            
            manager.CombatModule = new PlayerCombatModule(manager.CombatContext);
            manager.MoveModule = new PlayerMoveModule(manager.MoveContext);
            manager.IkModule = new PlayerIkModule(manager.IKContext);

            _lastPacketTime = Time.time;

            manager.CombatModule.Init();
            manager.MoveModule.Init();
            manager.IkModule.Init();
            
            InitTransformPosition("FnIws");
            manager.IkModule.SetMuzzle(_weapon.muzzle);
            manager.IkModule.SetHandIk(_weapon.hand);
            
            _playerFsm = new PlayerFsm(manager);

        }

        private void InitTransformPosition(string name)
        {
            var ir2 = ResourceManager.instance.Load
                ($"Assets/AssetBundle/ScriptableObject/Weapon/{name}/{name}.asset", false);
            var data = ir2.GetAsset<WeaponData>();
            IResource ir = ResourceManager.instance.Load($"Assets/AssetBundle/Model/Weapon/Remote/{data.weaponName}.prefab", false);
            GameObject prefab = Object.Instantiate(ir.GetAsset<GameObject>(), _m.gunCube, false);
            _weapon = prefab.GetComponent<WeaponPresenter>();
            var ir3 = ResourceManager.instance.Load(
                $"Assets/AssetBundle/ScriptableObject/Weapon/{data.name}/{name}_Trans.asset",false);
            var trans = ir3.GetAsset<TransformComponentData>();
            prefab.transform.localPosition = trans.pos;
            prefab.transform.localRotation = Quaternion.Euler(trans.rot.x, trans.rot.y, trans.rot.z);
            _m.CombatModule.RemoteWeapon = _weapon;
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
            UnpackFlatToState(dto);
            
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
                    _lastServerPos = newPos;
                    _isFirstPacket = false;
                }
                
                float timeSinceLastPacket = Time.time - _lastPacketTime;
                timeSinceLastPacket = Mathf.Max(timeSinceLastPacket, 0.01f);

                _currentVelocity = (newPos - _lastServerPos) / timeSinceLastPacket;
            
                _lastServerPos = newPos;
                _lastPacketTime = Time.time;
            }
        }
        
        private void UnpackFlatToState(PlayerDataFlat dto)
        {
            // 数值同步
            _m.state.curHp = dto.CurHp;
            _m.state.maxHp = dto.MaxHp;
            _m.state.curShield = dto.CurShield;
            _m.state.maxShield = dto.MaxShield;
        }

        public void OnUpdate()
        {
            _m.MoveModule.SyncFromState(_m.state);
            _m.CombatModule.SyncFromState(_m.state);
            _playerFsm.Update();
            _m.AnimController.Update();
            _m.IkModule?.OnUpdate();
            
            if (Time.time - _lastPacketTime > 0.2f)
            {
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, Time.deltaTime * 5f);
            }
            
        }

        public void OnClient50HzTick() { }
        public void OnClient20HzTick() { }

        public void OnLateUpdate() { }
    }
}