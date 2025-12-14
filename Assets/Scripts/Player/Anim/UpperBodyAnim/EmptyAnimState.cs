using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.UpperBodyAnim
{
    public class EmptyAnimState : PlayerAnimState
    {
        public EmptyAnimState(PlayerManager myManager) : base(myManager)
        {
            
        }

        protected override bool CheckState()
        {
            return (!MyManager.isReload && !MyManager.isThrow) || (MyManager.isThrow && MyManager.isRun);
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Clip = new AnimationClip();
            Clip.legacy = false;      // 必须非 legacy
            Clip.frameRate = 60;
            Clip.ClearCurves();       // 确保没有曲线

            AnimationClipPlayable emptyAnimState = AnimationClipPlayable.Create(graph, Clip);
            return emptyAnimState;
        }

        public override void OnExitState()
        {
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