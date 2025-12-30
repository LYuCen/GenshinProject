using Common.FSM.PlayerFSM;
using mSystem.HaemalStrandSystem;
using QFramework;
using UnityEngine;


namespace mController
{
    public class CameraController : MonoBehaviour, IController
    {
        public Transform playerTransform;
        [Header("射线长度")] public float rayLength;
        public Vector3 rayEnd; // 射击终点
        
        public PlayerManager manager;
        public float sensitivity = 2f;
        public float distance = 2f;
        public float aimDistance = 1f;
        public float height = 1.5f;
        public float minPitch = -30f;
        public float maxPitch = 60f;
        public float shoulderOffset = 0.2f; // 正数右肩，负数左肩

        private float _yaw;
        public float Yaw => _yaw;
        private float _pitch;
        private float _nowDistance;
        
        private IHaemalSystem _haemalSystem;
        private Ray _ray;
        private Camera _camera;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            Vector3 euler = transform.eulerAngles;
            _yaw = euler.y;  // XOZ平面与 z轴正方向偏转角
            _pitch = euler.x; // YOZ平面与 z轴正方向偏转角
            _nowDistance = distance; // 当前相机离角色距离
            _haemalSystem = this.GetSystem<IHaemalSystem>();
            _camera = Camera.main;
        }

        
        public float GetAngle() // 获取偏转角
        {
            Vector3 playerFront = playerTransform.forward;
            Vector2 front = new Vector2(playerFront.x, playerFront.z);
            Vector2 cameraFront = new Vector2(playerTransform.position.x - transform.position.x,
                playerTransform.position.z - transform.position.z);
            float dot = Vector2.Dot(front, cameraFront.normalized); // 点乘结果
            dot = Mathf.Clamp(dot, -1f, 1f);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // 转为角度制

            float cross = front.x * cameraFront.y - front.y * cameraFront.x; // 计算叉乘
            if (cross < 0) angle = -angle;
            return angle;
        }
    

        public void UpdateForward()
        {
            Shooting();
            _haemalSystem.UIUpdate();
        }
        

        void Shooting()
        {
            // 屏幕中心
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

            // 获取射线
            if(_camera) _ray = _camera.ScreenPointToRay(screenCenter);
            rayEnd = _ray.origin + _ray.direction * rayLength;
            
            if (Physics.Raycast(_ray, out RaycastHit hit, rayLength))
            {
                if (hit.collider.CompareTag("Plane") || hit.collider.CompareTag("Wall") ||
                    hit.collider.CompareTag("Enemy"))
                {
                    rayEnd = hit.point;
                }
            }
        }
        
        void LateUpdate()
        {
            _yaw += Input.GetAxis("Mouse X") * sensitivity;   // 获取鼠标 x输入
            _yaw = Mathf.Repeat(_yaw, 360f);
            _pitch -= Input.GetAxis("Mouse Y") * sensitivity; // 获取鼠标 y输入
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);  // 限制 y轴方向的范围

            Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0);

            _nowDistance += manager.isShoot ? -0.25f : 0.25f;
            _nowDistance = Mathf.Clamp(_nowDistance, aimDistance, distance);
            Vector3 targetPos = playerTransform.position + Vector3.up * height;
            Vector3 camPos = targetPos - rot * Vector3.forward * _nowDistance + rot * Vector3.right * shoulderOffset;
            
            transform.position = camPos;
            transform.rotation = rot;
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