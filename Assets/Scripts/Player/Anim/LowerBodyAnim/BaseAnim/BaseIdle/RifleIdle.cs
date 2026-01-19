using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseIdle
{
    public class RifleIdle : PlayerAnimState
    {
        public RifleIdle(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return !MyManager.isShoot;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override void OnExitState()
        {
            
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeLowerWeight(this);
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = Resources.Load<AnimConfig>("Anim/RifleIdle");
            AnimationClipPlayable rifleIdleClip = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleIdleClip.SetSpeed(Config.animSpeed);
            return rifleIdleClip;
        }
    }
}