using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes
{
    /// <summary>
    /// 持枪移动节点
    /// </summary>
    public class RifleWalkTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        
        public RifleWalkTreeLeafNode(PlayerManager myManager) : base(myManager){}

        public override bool CheckState(bool parentActive)
        {
            return parentActive && !MyMessage.IsShoot && !MyMessage.IsThrow;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalk.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}