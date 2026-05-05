using GameFSM.LevelFSM.EnemyFSM;
using mEnum;
using Player.Anim;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Enemy.Anim.LowerBodyAnim.CombatAnim
{
    public class EnemyCombatWalkF : EnemyAnimState
    {
        public EnemyCombatWalkF(EnemyManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return MyManager.enemyAIController.GetMoveDirection() == MoveDirection8.Forward;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Enemy/RifleWalkForward.asset");
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
    }
}