using Firearms.ThrowWeapon;
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public class HighThrowEndTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        private float _clipDuration;
        public HighThrowEndTreeLeafNode(PlayerManager myMessage) : base(myMessage)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            if (MyMessage.firearmController.Weapon is Grenade g)
            {
                return parentActive && g.IsThrowingAction;
            }

            return false;
        }
        
        public override void OnEnterState()
        {
            if (!_playable.IsValid()) return;
            // 只在 Throwing 阶段内部进行站/蹲的时间同步
            float savedTime = MyMessage.firearmController.currentActionNormalizedTime;
            _playable.SetTime(savedTime * _clipDuration);
        }

        public override float UpdateNode()
        {
            float w = base.UpdateNode();
            if (!_playable.IsValid()) return w;

            // 判定：只有当武器状态处于 Throwing 时，才同步时间
            if (w > 0.5f && MyMessage.firearmController.Weapon is Grenade g && g.CurrentState == GrenadeState.Throwing)
            {
                float normTime = (float)(_playable.GetTime() / _clipDuration);
                MyMessage.firearmController.currentActionNormalizedTime = normTime;

                // --- 逻辑分层触发 ---
                // 1. 0.7 时刻：只负责把手雷丢出去
                if (normTime >= 0.2f)
                {
                    g.ExecutePhysicalThrow();
                }

                // 2. 0.95 或更高：才负责把状态切回 Idle（这样动画才能播完）
                if (normTime >= 0.95f)
                {
                    g.OnThrowAnimationEnd();
                }
            }
            return w;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            // 直接使用基类里被解析器注入的变量
            AnimationClip clipToUse = UseTagOnly ? null : DefaultClip;

            if (!clipToUse && !string.IsNullOrEmpty(Tag))
            {
                // 去武器表里找
                clipToUse = MyMessage.firearmController.Weapon.GetClipByTag(Tag);
            }

            _playable = AnimationClipPlayable.Create(graph, clipToUse);
            _playable.SetSpeed(this.AnimSpeed); // 使用基类的 AnimSpeed
            
            if (clipToUse) _clipDuration = clipToUse.length;
            return _playable;
        }
    }
}