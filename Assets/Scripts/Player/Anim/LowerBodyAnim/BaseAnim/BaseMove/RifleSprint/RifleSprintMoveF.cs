using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove.RifleSprint
{
    public class RifleSprintMoveF : PlayerAnimState
    {
        public RifleSprintMoveF(PlayerManager myManager) : base(myManager)
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
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Sprint/RifleSprintF.asset");
            Clip = Config.animClip;
            AnimationClipPlayable rifleSprintMoveClipF = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleSprintMoveClipF.SetSpeed(Config.animSpeed);
            return rifleSprintMoveClipF;
        }

        public override void OnExitState()
        {
            
        }
        
        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeLowerWeight(this);
        }
    }
}