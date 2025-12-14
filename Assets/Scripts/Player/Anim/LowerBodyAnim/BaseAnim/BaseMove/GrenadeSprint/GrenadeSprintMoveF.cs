using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove.GrenadeSprint
{
    public class GrenadeSprintMoveF : PlayerAnimState
    {
        public GrenadeSprintMoveF(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return MyManager.inputMove == Vector2.up;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = Resources.Load<PlayerAnimConfig>("Anim/Grenade/GrenadeSprintF");
            AnimationClipPlayable grenadeSprintMoveClipF = AnimationClipPlayable.Create(graph, Config.animClip);
            grenadeSprintMoveClipF.SetSpeed(Config.animSpeed);
            return grenadeSprintMoveClipF;
        }

        public override void OnExitState()
        {
            
        }
        
        public override void NextState()
        {
            
        }
    }
}