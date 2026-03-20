using Common.LevelFSM.EnemyFSM;
using mEnum;
using Player.Anim;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Enemy.Anim.LowerBodyAnim.CombatAnim
{
    public class EnemyCombatWalkBL : EnemyAnimState
    {
        public EnemyCombatWalkBL(EnemyManager myManager) : base(myManager)
        {
        }
        
        protected override bool CheckState()
        {
            return MyManager.enemyAIController.GetMoveDirection() == MoveDirection8.BackLeft;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Enemy/RifleWalkBackwardLeft.asset");
            Clip = Config.animClip;
            AnimationClipPlayable rifleMoveClip = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleMoveClip.SetSpeed(Config.animSpeed);
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