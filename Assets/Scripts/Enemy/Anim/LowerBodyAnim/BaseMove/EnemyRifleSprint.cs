using Common.LevelFSM.EnemyFSM;
using mModel.ScriptableConfig;
using Player.Anim;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Enemy.Anim.LowerBodyAnim.BaseMove
{
    public class EnemyRifleSprint : EnemyAnimState
    {
        public EnemyRifleSprint(EnemyManager myManager) : base(myManager)
        {
        }
        
        protected override bool CheckState()
        {
            return MyManager.moveSpeed > 2.0f;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = Resources.Load<AnimConfig>("Anim/Rifle/Sprint/RifleSprintF");
            AnimationClipPlayable grenadeSprintMoveClipF = AnimationClipPlayable.Create(graph, Config.animClip);
            grenadeSprintMoveClipF.SetSpeed(Config.animSpeed);
            return grenadeSprintMoveClipF;
        }
        
        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.EnemyAnimController.ChangeLowerWeight(this);
        }
    }
}