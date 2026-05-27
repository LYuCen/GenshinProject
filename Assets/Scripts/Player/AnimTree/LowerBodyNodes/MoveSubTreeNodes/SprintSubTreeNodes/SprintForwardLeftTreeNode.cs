using GameFSM.LevelFSM.PlayerFSM;
using mEnum.Flags;
using mEnum.Query;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.SprintSubTreeNodes
{
    public class SprintForwardLeftTreeNode : AnimTreeLeafNode<PlayerManager>
    {
        public SprintForwardLeftTreeNode(PlayerManager myMessage) : base(myMessage)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            bool res = query.HasState(PlayerMoveFlags.ForwardLeft);
            return parentActive && res;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var playable = AnimationClipPlayable.Create(graph, DefaultClip);
            playable.SetSpeed(this.AnimSpeed); // 使用基类的 AnimSpeed
            return playable;
        }
    }
}