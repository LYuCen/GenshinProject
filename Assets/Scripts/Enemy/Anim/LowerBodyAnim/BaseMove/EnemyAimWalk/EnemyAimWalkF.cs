using Common.LevelFSM.EnemyFSM;
using Player.Anim;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Enemy.Anim.LowerBodyAnim.BaseMove.EnemyAimWalk
{
    public class EnemyAimWalkF : EnemyAnimState
    {
        public EnemyAimWalkF(EnemyManager myManager) : base(myManager)
        {
        }
        
        protected override bool CheckState()
        {
            return MyManager.moveSpeed < 2.0f;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkForward.asset");
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