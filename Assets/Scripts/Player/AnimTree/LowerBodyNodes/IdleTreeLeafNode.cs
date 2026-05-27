
using GameFSM.LevelFSM.PlayerFSM;
using mEnum.Flags;
using mEnum.Query;
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

        public IdleTreeLeafNode(PlayerManager myManager) : base(myManager)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();

            // 在地面且没有八方向移动
            bool res = query.HasState(PlayerMoveFlags.Ground) && !query.HasState(PlayerMoveFlags.Move);
            
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