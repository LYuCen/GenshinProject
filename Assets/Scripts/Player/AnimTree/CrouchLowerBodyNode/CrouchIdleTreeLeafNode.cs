using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.CrouchLowerBodyNode
{
    /// <summary>
    /// 蹲待机
    /// </summary>
    public class CrouchIdleTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        
        public CrouchIdleTreeLeafNode(PlayerManager myManager) : base(myManager){}

        public override bool CheckState(bool parentActive)
        { 
            return parentActive && MyMessage.IsGround && MyMessage.InputMove == Vector2.zero && !MyMessage.IsThrow;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Crouch/CrouchIdle.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}