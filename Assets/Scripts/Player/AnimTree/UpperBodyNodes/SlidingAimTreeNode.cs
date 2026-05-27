
using GameFSM.LevelFSM.PlayerFSM;
using mEnum.Flags;
using mEnum.Query;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes
{
    public class SlidingAimTreeNode : AnimTreeLeafNode<PlayerManager>
    {
        public SlidingAimTreeNode(PlayerManager myMessage) : base(myMessage)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            return parentActive && query.HasState(PlayerMoveFlags.Sliding);
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var playable = AnimationClipPlayable.Create(graph, DefaultClip);
            playable.SetSpeed(this.AnimSpeed); // 使用基类的 AnimSpeed
            return playable;
        }
    }
}