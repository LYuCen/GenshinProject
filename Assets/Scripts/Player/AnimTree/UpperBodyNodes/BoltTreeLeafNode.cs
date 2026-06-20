using IUtility;
using mEnum.Flags;
using mEnum.Query;
using mInterface;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes
{
    public class BoltTreeLeafNode : AnimTreeLeafNode<IAnimContext>
    {
        private AnimationClipPlayable _playable;
        private float _clipDuration;
        private float _handBlend;
        private bool _wasFullyActive;

        public BoltTreeLeafNode(IAnimContext ctx) : base(ctx)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            WeaponQuery query = MyMessage.CombatModule.GetQuery();

            bool res = query.HasState(WeaponFlags.Bolt);

            return parentActive && res;
        }

        public override void OnEnterState()
        {
            _playable.SetTime(0);
            if (CurrentData)
            {
                LateUpdate();
            }
        }

        public override float UpdateNode()
        {
            WeaponQuery query = MyMessage.CombatModule.GetQuery();
            bool shouldBeActive = query.HasState(WeaponFlags.Bolt);
            TargetWeight = shouldBeActive ? 1f : 0f;
            float currentWeight = base.UpdateNode();

            bool isFullyActive = currentWeight > 0.9f && shouldBeActive;
            if (isFullyActive && !_wasFullyActive && _playable.GetTime() >= _clipDuration)
            {
                _playable.SetTime(0);
            }
            _wasFullyActive = isFullyActive;

            if (isFullyActive)
            {
                if (_playable.GetTime() >= _clipDuration) { }
                else if (CurrentData) LateUpdate();
            }
            return currentWeight;
        }

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

        void LateUpdate()
        {
            float duration = _playable.GetAnimationClip().length;
            float t = (float)_playable.GetTime();
            float loopTime = Mathf.Clamp(t, 0, duration);

            float normalizedTime = loopTime / duration;

            if (CurrentData.handWeight != null && CurrentData.handWeight.length > 0)
            {
                _handBlend = CurrentData.handWeight.Evaluate(normalizedTime);
            }

            Vector3 rBP = CompressionUtility.GetPos(CurrentData.rPosX, CurrentData.rPosY, CurrentData.rPosZ, loopTime);
            Quaternion rBR = CompressionUtility.GetRot(CurrentData.rRotX, CurrentData.rRotY, CurrentData.rRotZ, loopTime);
            Vector3 lBP = CompressionUtility.GetPos(CurrentData.lPosX, CurrentData.lPosY, CurrentData.lPosZ, loopTime);
            Quaternion lBR = CompressionUtility.GetRot(CurrentData.lRotX, CurrentData.lRotY, CurrentData.lRotZ, loopTime);

            Vector3 finalRP = rBP + (rBR * CurrentData.rOffset);
            Quaternion finalRR = rBR * Quaternion.Euler(CurrentData.rRot);
            Vector3 finalLP = lBP + (lBR * CurrentData.lOffset);
            Quaternion finalLR = lBR * Quaternion.Euler(CurrentData.lRot);

            MyMessage.WeaponPivot.localPosition = Vector3.Lerp(finalLP, finalRP, _handBlend);
            MyMessage.WeaponPivot.localRotation = Quaternion.Slerp(finalLR, finalRR, _handBlend);
        }

        public override void OnExitState()
        {
            Debug.Log("Exit Reloading");
        }
    }
}
