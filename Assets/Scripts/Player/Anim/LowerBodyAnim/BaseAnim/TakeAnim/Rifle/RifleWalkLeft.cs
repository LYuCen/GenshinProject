using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.TakeAnim.Rifle
{
    public class RifleWalkLeft : PlayerAnimState
    {
        public RifleWalkLeft(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return MyManager.inputMove == Vector2.left;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = Resources.Load<PlayerAnimConfig>("Anim/RifleWalkLeft");
            AnimationClipPlayable rifleMoveClipL = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleMoveClipL.SetSpeed(Config.animSpeed);
            return rifleMoveClipL;
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