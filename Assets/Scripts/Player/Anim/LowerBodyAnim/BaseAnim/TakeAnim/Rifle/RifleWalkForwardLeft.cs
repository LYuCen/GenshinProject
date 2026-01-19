using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.TakeAnim.Rifle
{
    public class RifleWalkForwardLeft : PlayerAnimState
    {
        private Vector2 _vec = new Vector2(-1, 1);
        public RifleWalkForwardLeft(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return MyManager.inputMove == _vec;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = Resources.Load<AnimConfig>("Anim/RifleWalkForwardLeft");
            AnimationClipPlayable rifleMoveClipFL = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleMoveClipFL.SetSpeed(Config.animSpeed);
            return rifleMoveClipFL;
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