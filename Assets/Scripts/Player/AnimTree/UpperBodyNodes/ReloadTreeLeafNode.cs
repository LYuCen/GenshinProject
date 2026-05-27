using GameFSM.LevelFSM.PlayerFSM;
using IUtility;
using mEnum.Flags;
using mEnum.Query;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes
{
    /// <summary>
    /// 换弹节点
    /// </summary>
    public class ReloadTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        private float _clipDuration;
        private float _handBlend;

        private Vector3 rePositon;
        private Vector3 reRotation;

        public ReloadTreeLeafNode(PlayerManager manager) : base(manager) { }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            if (!DefaultClip)
            {
                Debug.LogError($"[ReloadNode] 找不到有效的动画剪辑！Tag: {Tag}");
                return Playable.Null;
            }
            
            _playable = AnimationClipPlayable.Create(graph, DefaultClip);
            
            _playable.SetSpeed(AnimSpeed);
            
            _clipDuration = DefaultClip.length;
            return _playable;
        }

        public override void OnEnterState()
        {
            Debug.Log("Start Reloading...");
            _playable.SetTime(0); // 每次进入时重置时间
            if (CurrentData)
            {
                Transform gun = MyMessage.CombatModule.GetWeapon();
                gun.SetParent(MyMessage.weaponPivot);
                gun.localPosition = Vector3.zero;
                gun.localEulerAngles = Vector3.zero;
                LateUpdate();
            }
        }
        
        public override bool CheckState(bool parentActive)
        {
            WeaponQuery query = MyMessage.CombatModule.GetQuery();
            
            bool res = query.HasState(WeaponFlags.Reload);
            
            return parentActive && res;
        }

        public override float UpdateNode()
        {
            WeaponQuery query = MyMessage.CombatModule.GetQuery();
            TargetWeight = query.HasState(WeaponFlags.Reload) ? 1f : 0f;
            float currentWeight = base.UpdateNode();
            if (currentWeight > 0.9f && query.HasState(WeaponFlags.Reload))
            {
                if (_playable.GetTime() >= _clipDuration)
                {
                }
                else if(CurrentData) LateUpdate();
            }
            return currentWeight;
        }
        
        void LateUpdate()
        {
            // 获取时长和绝对时间
            float duration = _playable.GetAnimationClip().length;
            float t = (float)_playable.GetTime();
            float loopTime = Mathf.Clamp(t, 0, duration);

            // 计算归一化时间 (0 到 1)
            float normalizedTime = loopTime / duration;

            // 手部权重曲线使用归一化时间 (0 - 1) 采样
            if (CurrentData.handWeight != null && CurrentData.handWeight.length > 0)
            {
                _handBlend = CurrentData.handWeight.Evaluate(normalizedTime);
                MyMessage.IkModule.SetHandIk(1 - _handBlend);
            }

            // 3. 烘焙曲线采样
            Vector3 rBP = CompressionUtility.GetPos(CurrentData.rPosX, CurrentData.rPosY, CurrentData.rPosZ, loopTime);
            Quaternion rBR = CompressionUtility.GetRot(CurrentData.rRotX, CurrentData.rRotY, CurrentData.rRotZ, loopTime);
            Vector3 lBP = CompressionUtility.GetPos(CurrentData.lPosX, CurrentData.lPosY, CurrentData.lPosZ, loopTime);
            Quaternion lBR = CompressionUtility.GetRot(CurrentData.lRotX, CurrentData.lRotY, CurrentData.lRotZ, loopTime);

            // 应用手部偏移
            Vector3 finalRP = rBP + (rBR * CurrentData.rOffset);
            Quaternion finalRR = rBR * Quaternion.Euler(CurrentData.rRot);
            Vector3 finalLP = lBP + (lBR * CurrentData.lOffset);
            Quaternion finalLR = lBR * Quaternion.Euler(CurrentData.lRot);
            
            MyMessage.weaponPivot.localPosition = Vector3.Lerp(finalLP, finalRP, _handBlend);
            MyMessage.weaponPivot.localRotation = Quaternion.Slerp(finalLR, finalRR, _handBlend);
        }

        public override void OnExitState()
        {
            MyMessage.CombatModule.ResetGun();
            MyMessage.IkModule.SetAimIk(MyMessage.GetAimIkTarget());
            MyMessage.IkModule.SetHandIk(MyMessage.GetHandIkTarget());
            Debug.Log("Exit Reloading");
        }
    }
}