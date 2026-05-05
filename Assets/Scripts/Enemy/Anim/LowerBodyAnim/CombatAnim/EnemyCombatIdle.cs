using GameFSM.LevelFSM.EnemyFSM;
using Player.Anim;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Enemy.Anim.LowerBodyAnim.CombatAnim
{
    public class EnemyCombatIdle : EnemyAnimState
    {
        public EnemyCombatIdle(EnemyManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return MyManager.moveSpeed < 0.2f;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Enemy/CombatIdle.asset");
            Clip = Config.animClip;
            AnimationClipPlayable rifleIdleClip = AnimationClipPlayable.Create(graph, Config.animClip);
            rifleIdleClip.SetSpeed(Config.animSpeed);
            return rifleIdleClip;
        }
        
        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.EnemyAnimController.ChangeLowerWeight(this);
        }
    }
}