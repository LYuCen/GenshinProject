using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove.RifleSprint
{
    public class RifleSprintMoveHeadR : PlayerAnimState
    {
        private PlayerAnimConfig _configR;
        private AnimationClipPlayable _sprintHead;
        
        public RifleSprintMoveHeadR(PlayerManager myManager) : base(myManager)
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
            _configR = Resources.Load<PlayerAnimConfig>("Anim/Rifle/Sprint/RifleSprintR");
            _sprintHead = AnimationClipPlayable.Create(graph, _configR.animClip);
            _sprintHead.SetSpeed(_configR.animSpeed);
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