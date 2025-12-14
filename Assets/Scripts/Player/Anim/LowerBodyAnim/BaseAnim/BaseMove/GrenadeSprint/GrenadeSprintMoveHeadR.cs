using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove.GrenadeSprint
{
    public class GrenadeSprintMoveHeadR : PlayerAnimState
    {
        private PlayerAnimConfig _configL;
        private AnimationClipPlayable _sprintHead;


        public GrenadeSprintMoveHeadR(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return true;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            _configL = Resources.Load<PlayerAnimConfig>("Anim/Grenade/GrenadeSprintR");
            _sprintHead = AnimationClipPlayable.Create(graph, _configL.animClip);
            _sprintHead.SetSpeed(_configL.animSpeed);
            return _sprintHead;
        }

        public override void NextState()
        {
            
        }

        public override void OnExitState()
        {
            
        }
    }
}