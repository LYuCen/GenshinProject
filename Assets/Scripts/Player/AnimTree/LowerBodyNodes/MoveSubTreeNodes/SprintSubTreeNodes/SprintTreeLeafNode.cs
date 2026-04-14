using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.SprintSubTreeNodes
{
    public class SprintTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        private string _animPath;

        // 修改后的构造函数
        public SprintTreeLeafNode(PlayerManager myManager, string animPath) : base(myManager)
        {
            _animPath = animPath;
        }

        public override bool CheckState(bool parentActive)
        {
            return parentActive;
        }

        // 修改后的 BuildNode
        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            var cfg = GetAnimConfig(_animPath);

            if (!cfg || !cfg.animClip)
            {
                Debug.LogError($"RifleSprintFTreeLeafNode: 无法加载配置，路径: {_animPath}");
                return Playable.Null;
            }

            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}