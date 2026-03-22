using Common.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    public class ShoulderShootWalkF : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        public ShoulderShootWalkF(PlayerManager myManager) : base(myManager){}

        public override bool CheckState(bool parentActive) => parentActive && MyManager.inputMove == Vector2.up;

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            // 路径推测: RifleWalkForward
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkForward.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}