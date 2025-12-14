using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.UpperBodyAnim
{
    public class ThrowAnimState : PlayerAnimState
    {
        private AnimationClipPlayable _throwPlayable;
        private bool _isReload;
        public ThrowAnimState(PlayerManager myManager) : base(myManager)
        {
            
        }

        protected override bool CheckState()
        {
            return !MyManager.firearmController.isReadyThrow;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Clip = Resources.Load<PlayerAnimConfig>("Anim/IdleThrowAnim").animClip;
            _throwPlayable = AnimationClipPlayable.Create(graph, Clip);
            return _throwPlayable;
        }

        public override void OnExitState()
        {
            _isReload = false;
            
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeUpperWeight(this);
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }
    }
}