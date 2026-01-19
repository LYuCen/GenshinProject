using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.TakeAnim.Rifle
{
    public class RifleWalkForward : PlayerAnimState
    {
        public RifleWalkForward(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return MyManager.inputMove == Vector2.up;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = Resources.Load<AnimConfig>("Anim/RifleWalkForward");
            AnimationClipPlayable rifleMoveClipF = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleMoveClipF.SetSpeed(Config.animSpeed);
            return rifleMoveClipF;
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