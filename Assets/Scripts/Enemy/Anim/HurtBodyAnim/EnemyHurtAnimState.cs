using Common.LevelFSM.EnemyFSM;
using Player.Anim;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Enemy.Anim.HitBodyAnim
{
    public class EnemyHurtAnimState : EnemyAnimState
    {
        public EnemyHurtAnimState(EnemyManager myManager) : base(myManager)
        {
        }
        
        protected override bool CheckState()
        {
            return MyManager.isHurt;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Hurt/HurtBody.asset");
            Clip = Config.animClip;
            AnimationClipPlayable hurtClip = AnimationClipPlayable.Create(graph, Config.animClip);
            hurtClip.SetSpeed(Config.animSpeed);
            return hurtClip;
        }
        
        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.EnemyAnimController.ChangeLowerWeight(this);
        }
    }
}