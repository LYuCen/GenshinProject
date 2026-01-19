using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove.RifleSprint
{
    public class RifleSprintMoveHeadL : PlayerAnimState
    {
        private AnimConfig _configL;
        private AnimationClipPlayable _sprintHead;
        
        public RifleSprintMoveHeadL(PlayerManager myManager) : base(myManager)
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
            _configL = Resources.Load<AnimConfig>("Anim/Rifle/Sprint/RifleSprintL");
            _sprintHead = AnimationClipPlayable.Create(graph, _configL.animClip);
            _sprintHead.SetSpeed(_configL.animSpeed);
            return _sprintHead;
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeLowerWeight(this);
        }

        public override void OnExitState()
        {
            
        }
    }
}