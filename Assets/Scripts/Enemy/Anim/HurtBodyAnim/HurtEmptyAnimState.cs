using GameFSM.LevelFSM.EnemyFSM;
using Player.Anim;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Enemy.Anim.HurtBodyAnim
{
    public class HurtEmptyAnimState : EnemyAnimState
    {
        public HurtEmptyAnimState(EnemyManager myManager) : base(myManager)
        {
        }

        protected override bool CheckState()
        {
            return !MyManager.isHurt;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Clip = new AnimationClip();
            Clip.legacy = false;      // 必须非 legacy
            Clip.frameRate = 60;
            Clip.ClearCurves();       // 确保没有曲线

            AnimationClipPlayable emptyAnimState = AnimationClipPlayable.Create(graph, Clip);
            return emptyAnimState;
        }

        public override void OnExitState()
        {
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.EnemyAnimController.ChangeUpperWeight(this);
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }
    }
}