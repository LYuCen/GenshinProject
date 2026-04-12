using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    public class CrouchShoulderShootWalkR : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
    
        public CrouchShoulderShootWalkR(PlayerManager myManager) : base(myManager){}

        public override bool CheckState(bool parentActive)
        {
            return parentActive && MyMessage.InputMove == Vector2.right;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            var cfg = GetAnimConfig(
                "Assets/AssetBundle/ScriptableObject/Anim/Rifle/Crouch/Walk/RifleCrouchWalkRight.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}