using mEvent.FiringRange;
using QFramework;
using ShootingTarget;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameFSM.FSM.PlayerTargetFSM
{
    public class PlayerTargetAI : MonoBehaviour, IController
    {
        [SerializeField] private PlayerTarget playerTarget;
        [SerializeField] private bool openMove;     // 开启移动
        [SerializeField] private bool openCrouch;   // 开启姿态切换 (蹲姿)

        [SerializeField] private float moveCd = 4f;
        [SerializeField] private float crouchCd = 3f;

        private float _nextMoveTime;
        private float _nextCrouchTime;

        private System.Action<TogglePlayerTargetMoveEvent> _onToggleMove;
        private System.Action<TogglePlayerTargetCrouchEvent> _onToggleCrouch;

        private void Awake()
        {
            _onToggleMove = _ =>
            {
                openMove = !openMove;
                if (!openMove) playerTarget.Data.inputMove.x = 0;
            };
            _onToggleCrouch = _ =>
            {
                openCrouch = !openCrouch;
            };
        }

        private void OnEnable()
        {
            this.RegisterEvent(_onToggleMove);
            this.RegisterEvent(_onToggleCrouch);
        }

        public void Init(bool toggleMove, bool toggleCrouch)
        {
            openMove = toggleMove;
            openCrouch = toggleCrouch;
        }
        

        private void Update()
        {
            if (playerTarget.transform.position.x > 5 || playerTarget.transform.position.x < -5)
            {
                playerTarget.Data.inputMove.x = playerTarget.transform.position.x > 5 ? 1 : -1;
                _nextMoveTime = Time.time + moveCd;
            }
            else if(openMove && Time.time > _nextMoveTime)
            {
                _nextMoveTime = Time.time + moveCd;
                RandomMove(playerTarget);
            }
            if (openCrouch && Time.time > _nextCrouchTime)
            {
                _nextCrouchTime = Time.time + crouchCd;
                RandomCrouch(playerTarget);
            }
        }
        
        private void OnDisable()
        {
            this.UnRegisterEvent(_onToggleMove);
            this.UnRegisterEvent(_onToggleCrouch);
        }

        /// <summary> 随机修改 inputMove.x 实现左右移动 </summary>
        private void RandomMove(PlayerTarget target)
        {
            if (target == null || target.Data == null) return;
            target.Data.inputMove.x = Random.Range(-1, 2);
        }

        /// <summary> 随机蹲下 </summary>
        private void RandomCrouch(PlayerTarget target)
        {
            if (target == null || target.Data == null) return;
            target.Data.isCrouch = Random.value > 0.5f;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}