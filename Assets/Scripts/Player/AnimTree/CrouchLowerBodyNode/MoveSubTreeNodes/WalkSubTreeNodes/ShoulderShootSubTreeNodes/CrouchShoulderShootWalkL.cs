using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    public class CrouchShoulderShootWalkL : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
    
        public CrouchShoulderShootWalkL(PlayerManager myManager) : base(myManager){}

        public override bool CheckState(bool parentActive)
        {
            return parentActive && MyMessage.InputMove == Vector2.left;
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

            var playable = AnimationClipPlayable.Create(graph, clipToUse);
            playable.SetSpeed(this.AnimSpeed); // 使用基类的 AnimSpeed
            return playable;
        }
    }
}