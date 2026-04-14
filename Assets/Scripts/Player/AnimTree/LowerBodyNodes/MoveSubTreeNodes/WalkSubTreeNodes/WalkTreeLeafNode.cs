using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes
{
    /// <summary>
    /// 持枪移动节点
    /// </summary>
    public class WalkTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        private string _animPath;

        // 修改后的构造函数：支持动态传入动画配置路径
        public WalkTreeLeafNode(PlayerManager myManager, string animPath) : base(myManager)
        {
            _animPath = animPath;
        }

        public override bool CheckState(bool parentActive)
        {
            return parentActive && !MyMessage.IsShoot;
        }

        // 修改后的 BuildNode：根据传入路径加载配置
        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            var cfg = GetAnimConfig(_animPath);
            
            if (!cfg || !cfg.animClip)
            {
                Debug.LogError($"RifleWalkTreeLeafNode: 无法加载配置或 Clip 为空，路径: {_animPath}");
                return Playable.Null;
            }

            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}