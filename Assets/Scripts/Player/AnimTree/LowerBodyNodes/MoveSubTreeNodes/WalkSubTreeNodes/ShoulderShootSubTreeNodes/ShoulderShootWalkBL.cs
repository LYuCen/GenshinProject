using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    public class ShoulderShootWalkBL : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        
        public ShoulderShootWalkBL(PlayerManager myManager) : base(myManager){}

        public override bool CheckState(bool parentActive)
        {
            return parentActive && MyMessage.InputMove == new Vector2(-1, -1);
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkBackwardLeft.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}