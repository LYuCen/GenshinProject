using Common.LevelFSM.EnemyFSM;
using mModel.ScriptableConfig;
using Player.Anim;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Enemy.Anim.LowerBodyAnim.BaseMove.EnemyAimWalk
{
    public class EnemyAimWalkR : EnemyAnimState
    {
        public EnemyAimWalkR(EnemyManager myManager) : base(myManager)
        {
        }
        
        protected override bool CheckState()
        {
            return MyManager.enemyAIController.GetMoveDir() > 0;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            AnimConfig config = Resources.Load<AnimConfig>("Anim/Enemy/RifleWalkRight");
            AnimationClipPlayable rifleMoveClip = AnimationClipPlayable.Create(graph, config.animClip);
            rifleMoveClip.SetSpeed(config.animSpeed);
            return rifleMoveClip;
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.EnemyAnimController.ChangeLowerWeight(this);
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }
    }
}