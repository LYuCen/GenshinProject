using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using IUtility;
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
            AnimationClip clipToUse = null;

            // 根据编辑器里的配置决定使用哪个 Clip
            if (UseTagOnly)
            {
                // 严格 Tag 模式：去当前武器里找
                if (MyMessage.firearmController.Weapon != null && !string.IsNullOrEmpty(Tag))
                {
                    clipToUse = MyMessage.firearmController.Weapon.GetClipByTag(Tag);
                }
            }
            else
            {
                // 混合模式：优先用图中拖进去的，没拖再用 Tag 找
                clipToUse = DefaultClip;
                if (!clipToUse && !string.IsNullOrEmpty(Tag))
                {
                    clipToUse = MyMessage.firearmController.Weapon?.GetClipByTag(Tag);
                }
            }

            if (!clipToUse)
            {
                Debug.LogError($"[ReloadNode] 找不到有效的动画剪辑！Tag: {Tag}");
                return Playable.Null;
            }
            
            _playable = AnimationClipPlayable.Create(graph, clipToUse);
            
            _playable.SetSpeed(AnimSpeed);
            
            _clipDuration = clipToUse.length;
            return _playable;
        }

        public override void OnEnterState()
        {
            Debug.Log("Start Reloading...");
            _playable.SetTime(0); // 每次进入时重置时间
            if (CurrentData)
            {
                MyMessage.playerGun.SetParent(MyMessage.firearmController.weaponPivot);
                MyMessage.playerGun.localPosition = Vector3.zero;
                MyMessage.playerGun.localEulerAngles = Vector3.zero;
                LateUpdate();
            }
        }
        
        public override bool CheckState(bool parentActive)
        {
            return (parentActive && MyMessage.IsReload) || Weight > 0.001f;
        }

        public override float UpdateNode()
        {
            TargetWeight = MyMessage.IsReload ? 1f : 0f;
            float currentWeight = base.UpdateNode();
            if (currentWeight > 0.9f && MyMessage.IsReload)
            {
                if (_playable.GetTime() >= _clipDuration)
                {
                    MyMessage.ReloadEnd(); // 这里会让 IsReload 变 false，触发上面的 TargetWeight = 0
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
                MyMessage.rHeadIkWeight = 1 - _handBlend;
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
            
            MyMessage.firearmController.weaponPivot.localPosition = Vector3.Lerp(finalLP, finalRP, _handBlend);
            MyMessage.firearmController.weaponPivot.localRotation = Quaternion.Slerp(finalLR, finalRR, _handBlend);
        }

        public override void OnExitState()
        {
            MyMessage.rHeadIkWeight = 1;
            MyMessage.ResetGun();
            
            Debug.Log("Exit Reloading");
        }
    }
}