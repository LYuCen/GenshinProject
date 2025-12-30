using mController;
using mSystem.BuffSystem.ElementBuff;
using mSystem.InputSystem.mEvent;
using mSystem.SoundSystem;
using Player.Anim;
using QFramework;
using UnityEngine;

namespace Common.FSM.PlayerFSM
{
    public class PlayerManager : MonoBehaviour, ICanRegisterEvent
    {
        [Header("当前状态")] public string stateName;
        [Header("上半身状态")] public string upperName;
        [Header("是否在地面")] public bool isGround;
        [Header("是否在跳跃")] public bool isJump;
        [Header("是否在射击")] public bool isShoot;
        [Header("是否在换弹")] public bool isReload;
        [Header("是否在奔跑")] public bool isRun;
        [Header("是否在投弹")] public bool isThrow;
        [Header("按键移动方位")] public Vector2 inputMove;
        [Header("角色移动方向")] public Vector2 playerMove;
        [Header("动画控制器")] public PlayAnim PlayAnimController;
        [Header("相机控制器")] public CameraController cameraController;
        [Header("移动速度")] public float speed;
        [Header("脚下判定框高度")] public float h;
        [Header("判定半径")] public float radius;
        [Header("重力")] public float g;
        [Header("受力")] public float f;
        [Header("组件")] public CharacterController characterController;
        [Header("角色动画组件")] public Animator animator;
        [Header("角色上下半身动画遮罩")] public AvatarMask avatarMask;

        [Header("手")] public Transform hand;
        [Header("左手ik")] public Transform leftHandIk;
        [Header("右手ik")] public Transform rightHandIk;
        [Header("枪")] public Transform playerGun;
        [Header("握把")] public Transform gunGrip;
        [Header("枪判定矩形")] public Transform gunCube;
        [Header("武器装备控制器")] public FirearmController firearmController;
        public Transform gunStock;
        public Transform gun; // 枪托
        public Transform shoulder; // 右肩
        public bool runEnable = true;
        private Ray _gunRay;
        private Vector3 _fV3; // 力方向
        private Vector3 _bottom;
        private Vector2 _lastMove; // 输入切换后移动方向
        private float _stableTimer = 0f; // 输入状态切换当前时间
        [Header("输入状态时间间隔")] public float stableThreshold = 0.05f; // 输入状态切换间隔
        
        public void Init()
        {
            animator.applyRootMotion = false;
            PlayAnimController = new PlayAnim(animator, this, avatarMask, leftHandIk, rightHandIk);
            playerGun = firearmController.GetWeapon(0);
            playerGun.SetParent(gunCube, true); // 保持世界位置/旋转/缩放
            gunCube.SetParent(hand, true);
            gunCube.localPosition = new Vector3(0.00016f, 0.00203f, 0.00078f);
            gunCube.localEulerAngles = new Vector3(-90, 180, 0);
            firearmController.Weapon.InitTransform();
            gunGrip = playerGun.Find("Grip");
            this.RegisterEvent<MoveEvent>(e =>
            {
                inputMove = e.MoveVec;
            });
            this.RegisterEvent<JumpEvent>(_ =>
            {
                if (isJump || !isGround) return; 
                //isJump = true;
            });
            this.RegisterEvent<WeaponSecondaryActionEvent>(e =>
            {
                if (e.IsDown)
                {
                    isShoot = !isShoot;
                    if (isShoot) isRun = false;
                    firearmController.DownSecondaryAction();
                }
            });
            this.RegisterEvent<WeaponPrimaryActionEvent>(e =>
            {
                isShoot = true;
                isRun = false;
                if(e.IsDown) firearmController.DownPrimaryAction();
                if(e.IsPress) firearmController.PressPrimaryAction();
                if(e.IsUp) firearmController.UpPrimaryAction();
            });
            this.RegisterEvent<ReloadEvent>(_ =>
            {
                if(firearmController.Weapon.ReloadTime() > 0) return;
                isReload = firearmController.TryReload();
            });
            this.RegisterEvent<MouseScrollEvent>(e =>
            {
                firearmController.ChangeElement(e.Scroll);
            });
            this.RegisterEvent<ChangeGunEvent>(e =>
            {
                if (firearmController.weaponIndex != e.GunIndex && firearmController.weaponChangeEnable)
                {
                    isReload = false;
                    isThrow = false;
                    if(e.GunIndex == 0)
                    {
                        playerGun = firearmController.GetWeapon(0);
                        SoundSystem.Instance.PlaySound("Assets/Resources/Sound/SoundEffect/FN_IWS/Get.mp3");
                        Debug.Log("切换为主武器1");
                    }
                    else if(e.GunIndex == 1)
                    {
                        playerGun = firearmController.GetWeapon(1);
                        SoundSystem.Instance.PlaySound("Assets/Resources/Sound/SoundEffect/MK14/Get.mp3");
                        Debug.Log("切换为主武器2");
                    }
                    isShoot = false;
                    if(playerGun == null) Debug.Log("武器为空");
                    playerGun.gameObject.SetActive(true);
                    SetParent(playerGun,gunCube);
                    firearmController.Weapon.InitTransform();
                    gunGrip = playerGun.Find("Grip");
                }
            });
            this.RegisterEvent<RunEvent>(_ =>
            {
                if (inputMove != Vector2.zero && runEnable)
                {
                    isRun = true;
                    isShoot = false;
                }
            });
            this.RegisterEvent<GetGrenade>(_ =>
            {
                isThrow = true;
                SoundSystem.Instance.PlaySound("Assets/Resources/Sound/SoundEffect/Grenade/Get.mp3");
                firearmController.UserThrow();
            });
        }

        public void SetParent(Transform child, Transform parent)
        {
            if (child.parent == parent) return;
            // 记录子物体的世界缩放
            Vector3 worldScale = child.lossyScale;

            // 设置父物体
            child.SetParent(parent);

            // 直接用保持世界缩放的方法，不需要手动除
            child.localScale = new Vector3(
                worldScale.x / parent.lossyScale.x,
                worldScale.y / parent.lossyScale.y,
                worldScale.z / parent.lossyScale.z
            );
        }


        private void Update()
        {
            if (isRun)
            {
                isRun = inputMove != Vector2.zero;
            }
            if (isReload && firearmController.Weapon.ReloadTime() == 0)
            {
                isReload = false;
                firearmController.Reload();
            }
            isGround = IsGrounded();
            if (!isGround)
            {
                _fV3.y = f * Time.deltaTime;
                transform.position += _fV3;
                if (f > g) f -= 2 * Time.deltaTime;
                isJump = false;
            }
            firearmController.UpdateForward((cameraController.rayEnd - gunStock.position).normalized);
            PlayAnimController.Update();
            PlayerMoveAnimContro();
            
        }

        private void LateUpdate()
        {
            cameraController.UpdateForward();
            if (isShoot)
            {
                _gunRay = new Ray(gunStock.position, (cameraController.rayEnd - gunStock.position).normalized);
                gun.position = shoulder.position;
                Vector3 forward = Vector3.Lerp(gun.forward, (cameraController.rayEnd - gunStock.position).normalized,
                    5 * Time.deltaTime);
                gun.forward = forward;
            }
        }

        bool IsGrounded()
        {
            _bottom = transform.position + Vector3.down * h;

            Collider[] hits = Physics.OverlapSphere(_bottom, radius);
            
            foreach (var col in hits)
            {
                if (col.gameObject == gameObject) continue; // 排除自己
                if (col.CompareTag("Plane"))
                    return true;
            }
            return false;
        }

        void PlayerMoveAnimContro() // 角色输入移动间隔控制
        {
            if (inputMove == _lastMove) // 输入向量和当前向量相同
            {
                _stableTimer += Time.deltaTime;

                if (_stableTimer >= stableThreshold) // 时间到达间隔是才允许切换移动动画
                {
                    playerMove = inputMove; // 输入稳定，更新动画用的向量
                }
            }
            else
            {
                // 输入发生变化：先重置计时器，但不立刻切换动画
                _stableTimer = 0f;
            }

            _lastMove = inputMove;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_bottom, radius);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(_gunRay.origin, cameraController.rayEnd);
        }

        private void OnDisable()
        {
            PlayAnimController.Destroy();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}