using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    public class ShoulderShootWalkL : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        public ShoulderShootWalkL(PlayerManager myManager) : base(myManager){}

        public override bool CheckState(bool parentActive) => parentActive && MyManager.InputMove == Vector2.left;

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            // 路径推测: RifleWalkLeft
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkLeft.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}