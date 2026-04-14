using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes
{
    /// <summary>
    /// 待机节点
    /// </summary>
    public class IdleTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        private string _animPath;

        public IdleTreeLeafNode(PlayerManager myManager, string animPath) : base(myManager)
        {
            _animPath = animPath;
        }

        public override bool CheckState(bool parentActive)
        { 
            return parentActive && MyMessage.IsGround &&
                   MyMessage.InputMove == Vector2.zero &&
                   !MyMessage.IsSliding;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            // 使用传入的路径加载配置
            var cfg = GetAnimConfig(_animPath);
            if (!cfg || !cfg.animClip)
            {
                Debug.LogError($"ReloadTreeLeafNode: 找不到动画配置 { _animPath}");
                return Playable.Null;
            }
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}