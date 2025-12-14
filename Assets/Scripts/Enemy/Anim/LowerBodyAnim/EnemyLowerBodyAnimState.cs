using Common.LevelFSM.EnemyFSM;
using Player.Anim;
using UnityEngine.Playables;

namespace Enemy.Anim.LowerBodyAnim
{
    public class EnemyLowerBodyAnimState : EnemyAnimState
    {
        public EnemyLowerBodyAnimState(EnemyManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return true;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            throw new System.NotImplementedException();
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            throw new System.NotImplementedException();
        }
    }
}