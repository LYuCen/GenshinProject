using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove
{
    public class RifleWalk : PlayerAnimState
    {
        public RifleWalk(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return !MyManager.isRun;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            PlayerAnimConfig config = Resources.Load<PlayerAnimConfig>("Anim/RifleWalk");
            AnimationClipPlayable rifleMoveClip = AnimationClipPlayable.Create(graph, config.animClip);
            rifleMoveClip.SetSpeed(config.animSpeed);
            return rifleMoveClip;
        }

        public override void OnExitState()
        {
            
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeLowerWeight(this);
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }
    }
}