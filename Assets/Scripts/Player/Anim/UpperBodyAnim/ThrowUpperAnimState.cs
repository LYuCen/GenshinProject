using Common.FSM.PlayerFSM;
using mEvent;
using QFramework;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.UpperBodyAnim
{
    public class ThrowUpperAnimState : PlayerAnimState, ICanRegisterEvent, ICanSendEvent
    {
        private AnimationClipPlayable _throwUpperPlayable;
        private double _animTime;
        private bool _throw;
        
        public ThrowUpperAnimState(PlayerManager myManager) : base(myManager)
        {
            this.RegisterEvent<ThrowEvent>(_ =>
            {
                if (!MyManager.firearmController.isDownThrow)
                {
                    MyManager.firearmController.weaponChangeEnable = false;
                    MyManager.runEnable = false;
                    MyManager.PlayAnimController.SetUpperWeightEnable(false);
                    _throw = true;
                    _throwUpperPlayable.SetTime(0);
                    _throwUpperPlayable.Play();
                }
            });
        }

        protected override bool CheckState()
        {
            return (MyManager.firearmController.isReadyThrow && !MyManager.firearmController.isDownThrow) || _throw;
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            PlayerAnimConfig config = Resources.Load<PlayerAnimConfig>("Anim/Grenade/GrenadeUpper");
            _throwUpperPlayable = AnimationClipPlayable.Create(graph, config.animClip);
            _throwUpperPlayable.SetSpeed(config.animSpeed);
            _throwUpperPlayable.Pause(); // 暂停动画
            _animTime = config.animClip.length;
            return _throwUpperPlayable;
        }

        public override void OnEnterState()
        {
            // 进入投掷手雷
            _throwUpperPlayable.SetTime(0);
            _throwUpperPlayable.SetTime(0);
            _throwUpperPlayable.Pause();
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeUpperWeight(this);

            double t = _throwUpperPlayable.GetTime();
            if (t >= _animTime + 0.02f)
            {
                MyManager.PlayAnimController.SetUpperWeightEnable(true);
                _throw = false;
            }
        }

        public override void OnExitState()
        {
            // 离开投掷手雷
            Debug.Log("离开投掷手雷");
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