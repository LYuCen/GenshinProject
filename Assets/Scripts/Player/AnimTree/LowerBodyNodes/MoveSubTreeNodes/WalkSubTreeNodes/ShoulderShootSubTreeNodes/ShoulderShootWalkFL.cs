using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    public class ShoulderShootWalkFL : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        public ShoulderShootWalkFL(PlayerManager myManager) : base(myManager){}

        public override bool CheckState(bool parentActive) => parentActive && MyManager.InputMove == new Vector2(-1, 1);

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            // 路径推测: RifleWalkForwardLeft
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkForwardLeft.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}