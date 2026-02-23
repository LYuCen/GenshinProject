using Common.FSM.PlayerFSM;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove.GrenadeSprint
{
    public class GrenadeSprintMoveHeadR : PlayerAnimState
    {
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
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Weapon/Grenade/GrenadeSprintR.asset");
            Clip = Config.animClip;
            _sprintHead = AnimationClipPlayable.Create(graph, Config.animClip);
            _sprintHead.SetSpeed(Config.animSpeed);
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