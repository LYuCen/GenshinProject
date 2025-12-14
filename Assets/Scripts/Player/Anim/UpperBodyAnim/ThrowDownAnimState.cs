using Common.FSM.PlayerFSM;
using mEvent;
using QFramework;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.UpperBodyAnim
{
    public class ThrowDownAnimState : PlayerAnimState, ICanRegisterEvent, ICanSendEvent
    {
        private AnimationClipPlayable _throwDownPlayable;
        private bool _throw;
        private double _animTime;
        
        
        public ThrowDownAnimState(PlayerManager myManager) : base(myManager)
        {
            this.RegisterEvent<ThrowEvent>(_ =>
            {
                if (MyManager.firearmController.isDownThrow)
                {
                    MyManager.runEnable = false;
                    MyManager.PlayAnimController.SetUpperWeightEnable(false);
                    MyManager.firearmController.weaponChangeEnable = false;
                    _throw = true;
                    _throwDownPlayable.SetTime(0);
                    _throwDownPlayable.Play();
                }
            });
        }

        protected override bool CheckState()
        {
            return (MyManager.firearmController.isReadyThrow && MyManager.firearmController.isDownThrow) || _throw;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            PlayerAnimConfig config = Resources.Load<PlayerAnimConfig>("Anim/Grenade/GrenadeDown");
            _throwDownPlayable = AnimationClipPlayable.Create(graph, config.animClip);
            _throwDownPlayable.SetSpeed(config.animSpeed);
            _throwDownPlayable.Pause(); // 暂停动画
            _animTime = config.animClip.length;
            return _throwDownPlayable;
        }
        
        public override void OnEnterState()
        {
            // 进入投掷手雷
            _throwDownPlayable.SetTime(0);
            _throwDownPlayable.SetTime(0);
            _throwDownPlayable.Pause();
        }
        
        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeUpperWeight(this);

            double t = _throwDownPlayable.GetTime();
            if (t >= _animTime)
            {
                MyManager.PlayAnimController.SetUpperWeightEnable(true);
                _throw = false;
            }
        }

        public override void OnExitState()
        {
            // 离开投掷手雷
            MyManager.firearmController.weaponChangeEnable = true;
            MyManager.runEnable = true;
            MyManager.firearmController.Weapon.SetEnable(true);
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}