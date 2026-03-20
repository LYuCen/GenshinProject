using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.UpperBodyAnim
{
    public class EmptyAnimState : PlayerAnimState
    {
        private AnimationClipPlayable _playable;
        private bool _initialized;
        public EmptyAnimState(PlayerManager myManager) : base(myManager)
        {
            
        }

        protected override bool CheckState()
        {
            //return (!MyManager.isReload && !MyManager.isThrow) || (MyManager.isThrow && MyManager.isRun);
            return !MyManager.isReload;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            
            Clip = new AnimationClip(); // 或者一个 idleUpperClip

            var playable = AnimationClipPlayable.Create(graph, Clip);
            playable.SetSpeed(0);

            _playable = playable;

            return playable;
        }

        public override void OnExitState()
        {
        }

        public override void NextState()
        {
            if (!CheckState()) return;

            if (!_initialized)
            {
                var reloadState = MyManager.ReloadState;

                if (reloadState != null && reloadState.HasPlayable())
                {
                    var time = reloadState.GetCurrentTime();
                    var clip = reloadState.GetClip();

                    var graph = _playable.GetGraph();
                    var newPlayable = AnimationClipPlayable.Create(graph, clip);

                    newPlayable.SetTime(time);
                    newPlayable.SetSpeed(0);

                    Parent.SetAnimClip(newPlayable, MixIndex);
                    _playable = newPlayable;
                }

                _initialized = true; // ✅ 只执行一次
            }

            MyManager.PlayAnimController.ChangeUpperWeight(this);
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }
    }
}