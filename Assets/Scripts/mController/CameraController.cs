using AI;
using AI.VisualRequest.VisualRequest;
using GameFSM.LevelFSM.PlayerFSM;
using mEnum.Flags;
using mEnum.Query;
using QFramework;
using UnityEngine;


namespace mController
{
    public class CameraController : MonoBehaviour, IController
    {
        public Transform playerTransform;
        [Header("射线长度")] public float rayLength;
        [Header("相机碰撞白名单")] [SerializeField] private LayerMask obstacleMask;
        [Header("相机碰撞阈值")] [SerializeField] private float wallOffset = 0.1f;
        [Header("枪口上抬偏移量")] public float liftOffset = 2f; // 往撞击点上方偏移多少
        [Header("平滑过渡速度")] public float smoothSpeed = 10f;
        public Transform scopeCamera; // 瞄准镜相机
        public Vector3 rayEnd; // 射击终点
        
        public PlayerManager manager;
        public float sensitivity = 2f;  // 灵敏度
        public float distance = 2f;     // 摄像机与角色距离
        public float aimDistance = 1f;  // 瞄准距离
        public float height = 1.5f;     // 摄像机与焦点高度
        public float minPitch = -30f;   // y 方向最小偏移
        public float maxPitch = 60f;    // y 方向最大偏移
        public float shoulderOffset = 0.2f; // 正数右肩，负数左肩
        public float endDistance = 2f;

        private float _yaw;
        public float Yaw => _yaw;   // XOZ平面与 z轴正方向偏转角
        public float Pitch => _pitch;
        private float _pitch;       // YOZ平面与 z轴正方向偏转角
        private float _nowDistance; // 当前相机离角色距离
        
        private Ray _ray;
        private Camera _camera;
        private int _targetMask;
        private int _originalMask;
        private int _headLayerIndex;
        PointVisualRequest _request = new PointVisualRequest();
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            Vector3 euler = transform.eulerAngles;
            _yaw = euler.y;  
            _pitch = euler.x; 
            _nowDistance = distance;
            _camera = Camera.main;
            _targetMask = LayerMask.GetMask("Default", "Hitbox", "Plane", "Wall");

            if (_camera != null)
            {
                _originalMask = _camera.cullingMask;
                _headLayerIndex = LayerMask.NameToLayer("Head");
            }

            _request.Radius = 0.01f;

            VisualizerManager.Instance.Register(_request);
        }
    

        public void UpdateForward()
        {
            Shooting();
        }

        void FixedUpdate()
        {
            PlayerMoveQuery query = manager.MoveModule.GetQuery();
            if (query.HasState(PlayerMoveFlags.Crouch))
            {
                height = Mathf.MoveTowards(height, 1.2f, Time.fixedDeltaTime);
            }
            else height = Mathf.MoveTowards(height, 1.5f, Time.fixedDeltaTime);
        }


        void Shooting()
        {
            // 屏幕中心
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

            // 获取射线
            if(_camera) _ray = _camera.ScreenPointToRay(screenCenter);
            rayEnd = _ray.origin + _ray.direction * rayLength;
            
            if (Physics.Raycast(_ray, out RaycastHit hit, rayLength, _targetMask))
            {
                float dis = (hit.point - manager.gunStock.position).magnitude;
                // 墙到相机的真实距离
                if (dis < endDistance * endDistance)
                {
                    // 如果离墙过近就指向远方
                    rayEnd = _ray.origin + _ray.direction * rayLength;
                }
                else
                {
                    // 距离足够远，正常指向撞击点
                    rayEnd = hit.point;
                }
            }
            else
            {
                rayEnd = _ray.origin + _ray.direction * rayLength;
            }

            _request.PointPos = rayEnd;
        }
        
        void LateUpdate()
        {
            _yaw += Input.GetAxis("Mouse X") * sensitivity;   // 获取鼠标 x输入
            _yaw = Mathf.Repeat(_yaw, 360f);
            _pitch -= Input.GetAxis("Mouse Y") * sensitivity; // 获取鼠标 y输入
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);  // 限制 y轴方向的范围

            Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0);

            WeaponQuery query = manager.CombatModule.GetQuery();

            if (query.HasState(WeaponFlags.Shoot) && !query.HasState(WeaponFlags.Shoulder)
                                                  && !query.HasState(WeaponFlags.Throw)
                                                  && !query.HasState(WeaponFlags.Blot))
            {
                // 瞄准视角
                transform.position = scopeCamera.position;
                transform.rotation = rot;
                _camera.cullingMask = _originalMask & ~(1 << _headLayerIndex); // 开镜时取消渲染头部
                if(scopeCamera) scopeCamera.gameObject.SetActive(true);
            }
            else
            {
                // 第三人称视角
                _nowDistance += query.HasState(WeaponFlags.Shoulder) ? -0.25f : 0.25f;
                _nowDistance = Mathf.Clamp(_nowDistance, aimDistance, distance);
                Vector3 targetPos = playerTransform.position + Vector3.up * height;
                Vector3 camPos = targetPos - rot * Vector3.forward * _nowDistance + rot * Vector3.right * shoulderOffset;
            
                //transform.position = camPos;
                CheckWall(targetPos, camPos);
                transform.rotation = rot;
                
                _camera.cullingMask = _originalMask; // 恢复头部渲染
                if(scopeCamera) scopeCamera.gameObject.SetActive(false);
            }
        }
        
        private void CheckWall(Vector3 targetPos, Vector3 camPos)
        {
            Vector3 rayVec = camPos - targetPos;
            float dist = rayVec.magnitude;
            Vector3 dir = rayVec.normalized;

            Ray ray = new Ray(targetPos, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, dist, obstacleMask))
            {
                // 被墙挡住，相机放在碰撞点前一点
                transform.position = hit.point - dir * wallOffset;
            }
            else
            {
                // 没有障碍物，直接使用理想相机位置
                transform.position = camPos;
            }

            // Debug 可视化
            Debug.DrawRay(targetPos, dir * dist, Color.yellow);
        }


        private void OnDrawGizmos()
        {
            if (_ray.direction != Vector3.zero)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(_ray.origin, rayEnd);
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}