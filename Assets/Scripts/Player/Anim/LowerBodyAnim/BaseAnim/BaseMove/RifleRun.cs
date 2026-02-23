using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove
{
    public class RifleRun : PlayerAnimState
    {
        public RifleRun(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return MyManager.isRun && !MyManager.isThrow;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Run/RifleRun.asset");
            Clip = Config.animClip;
            AnimationClipPlayable rifleRunClip = AnimationClipPlayable.Create(graph, Clip);
            rifleRunClip.SetSpeed(Config.animSpeed);
            return rifleRunClip;
        }

        public override void OnExitState()
        {
            
        }
        
        public override void NextState()
        {
            if (!CheckState()) return;
            Debug.Log("基础跑步");
            MyManager.PlayAnimController.ChangeLowerWeight(this);
        }
    }
}