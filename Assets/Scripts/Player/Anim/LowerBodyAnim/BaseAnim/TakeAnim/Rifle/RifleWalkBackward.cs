using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.TakeAnim.Rifle
{
    public class RifleWalkBackward : PlayerAnimState
    {
        public RifleWalkBackward(PlayerManager myManager) : base(myManager)
        {
            
        }

        protected override bool CheckState()
        {
            return MyManager.inputMove == Vector2.down;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkBackward.asset");
            Clip = Config.animClip;
            AnimationClipPlayable rifleMoveClipB = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleMoveClipB.SetSpeed(Config.animSpeed);
            return rifleMoveClipB;
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