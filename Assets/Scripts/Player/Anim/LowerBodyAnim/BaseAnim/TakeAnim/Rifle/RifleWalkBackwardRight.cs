using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.TakeAnim.Rifle
{
    public class RifleWalkBackwardRight : PlayerAnimState
    {
        private Vector2 _vec = new Vector2(1, -1);
        public RifleWalkBackwardRight(PlayerManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return MyManager.inputMove == _vec;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkBackwardRight.asset");
            Clip = Config.animClip;
            AnimationClipPlayable rifleMoveClipBR = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleMoveClipBR.SetSpeed(Config.animSpeed);
            return rifleMoveClipBR;
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