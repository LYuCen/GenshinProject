using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.ThrowAnim.BaseMove
{
    public class ThrowBaseWalk : PlayerAnimState
    {
        public ThrowBaseWalk(PlayerManager myManager) : base(myManager)
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
            Clip = Resources.Load<AnimConfig>("Anim/ThrowAnim/ThrowWalkAnim").animClip;
            AnimationClipPlayable throwWalkIdle = AnimationClipPlayable.Create(graph, Clip);
            return throwWalkIdle;
        }

        public override void OnExitState()
        {
            
        }
    }
}