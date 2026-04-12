using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes
{
    public class CrouchRifleWalkTreeLeafNode : AnimTreeMixNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        
        public CrouchRifleWalkTreeLeafNode(PlayerManager myManager) : base(myManager){}

        public override bool CheckState(bool parentActive)
        {
            return parentActive && !MyMessage.IsShoot && !MyMessage.IsThrow;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            var cfg = GetAnimConfig
                ("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Crouch/Walk/RifleCrouchWalk.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}