using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    public class ShoulderShootWalkR : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        public ShoulderShootWalkR(PlayerManager myManager) : base(myManager){}

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.InputMove == Vector2.right;

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            // 路径推测: RifleWalkRight
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkRight.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}