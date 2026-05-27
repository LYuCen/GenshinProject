using Firearms.ThrowWeapon;
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public class HighThrowStartTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        private float _clipDuration;
        
        public HighThrowStartTreeLeafNode(PlayerManager myMessage) : base(myMessage)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            /*
            if (MyMessage.firearmController.Weapon is Grenade g)
            {
                return parentActive && g.IsPullingPin;
            }
            */

            return false;
        }

        public override void OnEnterState()
        {
            /*
            if (!_playable.IsValid()) return;
            // 继承进度：如果是从“站立拉环”切到“蹲下拉环”，它们共用 PullingPin 阶段的进度
            float savedTime = MyMessage.firearmController.currentActionNormalizedTime;
            _playable.SetTime(savedTime * _clipDuration);
            */
        }

        public override float UpdateNode()
        {
            float w = base.UpdateNode();
            if (!_playable.IsValid()) return w;

            float normTime = (float)(_playable.GetTime() / _clipDuration);

            /*
            if (w > 0.5f && MyMessage.firearmController.Weapon is Grenade g && g.CurrentState == GrenadeState.PullingPin)
            {
                MyMessage.firearmController.currentActionNormalizedTime = normTime;
            }

            // 判定拉环结束
            if (normTime >= 0.98f) 
            {
                var grenade = MyMessage.firearmController.Weapon as Grenade;
                grenade?.OnPullPinFinish(); 
            }
            */
            return w;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            _playable = AnimationClipPlayable.Create(graph, DefaultClip);
            _playable.SetSpeed(this.AnimSpeed); // 使用基类的 AnimSpeed

            if (DefaultClip) _clipDuration = DefaultClip.length;
            return _playable;
        }
    }
}