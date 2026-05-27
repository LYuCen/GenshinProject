using GameFSM.LevelFSM.PlayerFSM;
using mEnum.Flags;
using mEnum.Query;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.SprintSubTreeNodes
{
    public class SprintForwardTreeNode : AnimTreeLeafNode<PlayerManager>
    {
        public SprintForwardTreeNode(PlayerManager myMessage) : base(myMessage)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            bool res = query.HasState(PlayerMoveFlags.Forward);
            return parentActive && res;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            // 直接使用基类里被解析器注入的变量
            var playable = AnimationClipPlayable.Create(graph, DefaultClip);
            playable.SetSpeed(this.AnimSpeed); // 使用基类的 AnimSpeed
            return playable;
        }
    }
}