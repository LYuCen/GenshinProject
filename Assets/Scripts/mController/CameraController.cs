using System.Collections.Generic;
using Cinemachine;
using Common.FSM.PlayerFSM;
using mSystem.HaemalStrandSystem;
using mSystem.InputSystem.mEvent;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;


namespace mController
{
    public class CameraController : MonoBehaviour, IController
    {
        public Transform playerTransform;
        public Transform cameraTransform;
        public Transform viewTransform;
        [Header("视野偏移")] public float distance;
        public Vector3 camForward;
        public Vector3 camRight;
        [Header("射线长度")] public float rayLength;
        public Vector3 rayEnd; // 射击终点
        public Dictionary<(int, int), float> Direction; // 八向图
        public PlayerManager playerManager;
        public CinemachineFreeLook baseCinema;
        public CinemachineFreeLook shootCinema;
        
        private IHaemalSystem _haemalSystem;
        private Ray _ray;
        private Camera _camera;
        
        private void Start()
        {
            Direction = new Dictionary<(int, int), float>();
            Direction.TryAdd((0, 0), 0);
            Direction.TryAdd((0, 1), 0);
            Direction.TryAdd((1, 1), 45);
            Direction.TryAdd((1, 0), 90);
            Direction.TryAdd((1, -1), 135);
            Direction.TryAdd((0, -1), 180);
            Direction.TryAdd((-1, -1), -135);
            Direction.TryAdd((-1, 0), -90);
            Direction.TryAdd((-1, 1), -45);

            _haemalSystem = this.GetSystem<IHaemalSystem>();
            _camera = Camera.main;
        }

        public float GetAngle() // 获取偏转角
        {
            Vector3 playerFront = playerTransform.forward;
            Vector2 front = new Vector2(playerFront.x, playerFront.z);
            Vector2 cameraFront = new Vector2(playerTransform.position.x - cameraTransform.position.x,
                playerTransform.position.z - cameraTransform.position.z);
            float dot = Vector2.Dot(front, cameraFront.normalized); // 点乘结果
            dot = Mathf.Clamp(dot, -1f, 1f);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // 转为角度制

            float cross = front.x * cameraFront.y - front.y * cameraFront.x; // 计算叉乘
            if (cross < 0) angle = -angle;
            return angle;
        }

        public void UpdateForward()
        {
            camForward = playerTransform.position - cameraTransform.position;
            camForward.y = 0;
            camForward.Normalize();
            camRight = new Vector3(camForward.z, 0, -camForward.x);
            viewTransform.position = playerTransform.position + camRight * distance;
            Shooting();
            _haemalSystem.UIUpdate();
        }

        public void AwakeCinemachine(CinemachineFreeLook from, CinemachineFreeLook to)
        {
            // 同步角度状态
            to.m_XAxis.Value = from.m_XAxis.Value;
            to.m_YAxis.Value = from.m_YAxis.Value;
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