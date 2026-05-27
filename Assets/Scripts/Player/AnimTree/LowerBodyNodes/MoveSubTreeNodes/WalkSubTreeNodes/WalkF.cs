using GameFSM.LevelFSM.PlayerFSM;
using mEnum.Flags;
using mEnum.Query;
using Player.AnimPro;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes
{
    public class WalkF : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
    
        public WalkF(PlayerManager myManager) : base(myManager){}

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();
            bool res = query.HasState(PlayerMoveFlags.Forward);
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