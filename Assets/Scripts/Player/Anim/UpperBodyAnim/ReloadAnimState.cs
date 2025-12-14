using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.UpperBodyAnim
{
    public class ReloadAnimState : PlayerAnimState
    {
        private AnimationClipPlayable _reloadPlayable;
        private bool _isReload;
        public ReloadAnimState(PlayerManager myManager) : base(myManager)
        {
            
        }

        protected override bool CheckState()
        {
            return MyManager.isReload;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Clip = Resources.Load<PlayerAnimConfig>("Anim/ReloadAnim").animClip;
            _reloadPlayable = AnimationClipPlayable.Create(graph, Clip);
            _reloadPlayable.Pause(); // 暂停动画
            return _reloadPlayable;
        }

        public override void OnExitState()
        {
            _isReload = false;
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeUpperWeight(this);
            
            // 每次切到 Reload 动画时，从头开始
            if (!_isReload)
            {
                _reloadPlayable.SetTime(0);
                _reloadPlayable.SetTime(0);
                _reloadPlayable.Play();
                _isReload = true;
            }
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }
    }
}