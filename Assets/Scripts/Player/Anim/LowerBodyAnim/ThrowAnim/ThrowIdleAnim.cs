using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.ThrowAnim
{
    public class ThrowIdleAnim : PlayerAnimState
    {
        public ThrowIdleAnim(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return !MyManager.isShoot;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Clip = Resources.Load<AnimConfig>("Anim/ThrowAnim/ThrowIdleAnim").animClip;
            AnimationClipPlayable throwIdle = AnimationClipPlayable.Create(graph, Clip);
            return throwIdle;
        }

        public override void OnExitState()
        {
            
        }
    }
}