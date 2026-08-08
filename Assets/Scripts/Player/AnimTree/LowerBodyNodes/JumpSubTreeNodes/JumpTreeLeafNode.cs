using mEnum.Flags;
using mEnum.Query;
using mInterface;
using Player.AnimPro;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.JumpSubTreeNodes
{
    public class JumpTreeLeafNode : AnimTreeLeafNode<IAnimContext>
    {
        private Playable _playable;
        
        public JumpTreeLeafNode(IAnimContext myMessage) : base(myMessage)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            PlayerMoveQuery query = MyMessage.MoveModule.GetQuery();

            // 在跳跃状态
            bool res = query.HasState(PlayerMoveFlags.Jump) && !query.HasState(PlayerMoveFlags.Ground);

            return parentActive && res;
        }
        
        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            _playable = AnimationClipPlayable.Create(graph, DefaultClip);
            _playable.SetSpeed(this.AnimSpeed);
            return _playable;
        }

        public override void OnEnterState()
        {
            _playable.SetTime(0);
        }
    }
}