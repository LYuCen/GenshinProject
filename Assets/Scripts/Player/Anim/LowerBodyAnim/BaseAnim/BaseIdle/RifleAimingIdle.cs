using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseIdle
{
    public class RifleAimingIdle : PlayerAnimState
    {
        public RifleAimingIdle(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return MyManager.isShoot && MyManager.playerMove == Vector2.zero;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override void OnExitState()
        {
            
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeLowerWeight(this);
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Idle/RifleAimingIdle.asset");
            Clip = Config.animClip;
            AnimationClipPlayable rifleAimIdle = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleAimIdle.SetSpeed(Config.animSpeed);
            return rifleAimIdle;
        }
    }
}