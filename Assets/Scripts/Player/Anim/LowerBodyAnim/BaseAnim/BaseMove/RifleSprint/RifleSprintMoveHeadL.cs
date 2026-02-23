using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove.RifleSprint
{
    public class RifleSprintMoveHeadL : PlayerAnimState
    {
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
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Sprint/RifleSprintL.asset");
            Clip = Config.animClip;
            _sprintHead = AnimationClipPlayable.Create(graph, Config.animClip);
            _sprintHead.SetSpeed(Config.animSpeed);
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