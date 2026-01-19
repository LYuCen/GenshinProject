using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.TakeAnim.Rifle
{
    public class RifleWalkBackward : PlayerAnimState
    {
        public RifleWalkBackward(PlayerManager myManager) : base(myManager)
        {
            
        }

        protected override bool CheckState()
        {
            return MyManager.inputMove == Vector2.down;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            AnimConfig config = Resources.Load<AnimConfig>("Anim/RifleWalkBackward");
            AnimationClipPlayable rifleMoveClipB = AnimationClipPlayable.Create(graph, config.animClip);
            rifleMoveClipB.SetSpeed(config.animSpeed);
            return rifleMoveClipB;
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