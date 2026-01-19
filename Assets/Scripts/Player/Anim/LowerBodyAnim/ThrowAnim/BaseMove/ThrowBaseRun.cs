using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.ThrowAnim.BaseMove
{
    public class ThrowBaseRun : PlayerAnimState
    {
        public ThrowBaseRun(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return !MyManager.isRun;
        }

        public override void SetWeight(int index, float weight = 0)
        {

        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Clip = Resources.Load<AnimConfig>("Anim/Grenade/GrenadeSprint").animClip;
            AnimationClipPlayable throwRunIdle = AnimationClipPlayable.Create(graph, Clip);
            return throwRunIdle;
        }

        public override void OnExitState()
        {

        }
    }
}