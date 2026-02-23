using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.TakeAnim.Rifle
{
    public class RifleWalkRight : PlayerAnimState
    {
        public RifleWalkRight(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return MyManager.inputMove == Vector2.right;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkRight.asset");
            Clip = Config.animClip;
            AnimationClipPlayable rifleMoveClipR = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleMoveClipR.SetSpeed(Config.animSpeed);
            return rifleMoveClipR;
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