using Common.LevelFSM.EnemyFSM;
using Player.Anim;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Enemy.Anim.LowerBodyAnim.BaseIdle
{
    public class EnemyRifleIdle : EnemyAnimState
    {
        public EnemyRifleIdle(EnemyManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return MyManager.moveSpeed < 0.1f;
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
            MyManager.EnemyAnimController.ChangeLowerWeight(this);
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = Resources.Load<PlayerAnimConfig>("Anim/RifleIdle");
            AnimationClipPlayable rifleIdleClip = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleIdleClip.SetSpeed(Config.animSpeed);
            return rifleIdleClip;
        }
    }
}