using mEnum;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private int moveOri; // 移动方位

        private void Start()
        {
            navMeshAgent.updateRotation = false;
        }

        public NavMeshAgent GetAgent()
        {
            return navMeshAgent;
        }

        /// <summary>
        /// 目标是否在范围内
        /// </summary>
        /// <param name="aim">目标</param>
        /// <param name="range">范围半径</param>
        /// <returns></returns>
        public bool IsInRange(Transform aim,float range)
        {
            if (!aim) return false;
            Vector3 dic = aim.position - transform.position;
            float distance = dic.sqrMagnitude;
            return distance < range * range;
        }

        /// <summary>
        /// 移动到目标地点
        /// </summary>
        /// <param name="aim">目标</param>
        /// <param name="stopPrevAgent">是否强制前往目标</param>
        public void MoveToAim(Vector3 aim, bool stopPrevAgent = false)
        {
            if (stopPrevAgent || !navMeshAgent.hasPath) // 强制打断 或 不存在有效路径
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(aim);
            }
        }
        
        /// <summary>
        /// 停止 AI 寻路
        /// </summary>
        public void StopMove()
        {
            if (!navMeshAgent) return;

            navMeshAgent.isStopped = true;   // 停止移动
            navMeshAgent.ResetPath();        // 清空当前路径
        }
        
        public void ChangeMoveSpeed(float speed)
        {
            navMeshAgent.speed = speed;
        }
        
        /// <summary>
        /// 获得当前速度
        /// </summary>
        /// <returns></returns>
        public float GetCurrentSpeed()
        {
            if (!navMeshAgent) return 0f;
            return navMeshAgent.velocity.magnitude;
        }

        /// <summary>
        /// 获得当前位移方向
        /// </summary>
        /// <returns></returns>
        public int GetMoveDir()
        {
            return moveOri;
        }
        
        /// <summary>
        /// 是否以到达目标点位
        /// </summary>
        /// <returns></returns>
        public bool HasArrived()
        {
            if (navMeshAgent.pathPending) return false;

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 朝向移动方向（平滑旋转）
        /// </summary>
        /// <param name="rotateSpeed">旋转速度</param>
        public void RotateToMoveDirection(float rotateSpeed)
        {
            if (!navMeshAgent) return;

            Vector3 velocity = navMeshAgent.velocity;

            // 如果速度很小，不旋转
            if (velocity.sqrMagnitude < 0.01f)
                return;

            // 计算目标方向
            Vector3 dir = velocity.normalized;

            Quaternion targetRot = Quaternion.LookRotation(dir);

            // 平滑旋转
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );
        }
        
        public void RotateToMoveDirection(float rotateSpeed, Vector3 forward)
        {
            if (!navMeshAgent) return;

            forward.y = 0;

            if (forward.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRot = Quaternion.LookRotation(forward);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );
        }
        
        /// <summary>
        /// 获取8方向移动
        /// </summary>
        public MoveDirection8 GetMoveDirection()
        {
            if (!navMeshAgent) return MoveDirection8.Forward;

            Vector3 velocity = navMeshAgent.velocity;

            if (velocity.sqrMagnitude < 0.01f)
                return MoveDirection8.Forward;

            // 转换到角色本地坐标
            Vector3 localVelocity = transform.InverseTransformDirection(velocity).normalized;

            float x = localVelocity.x;
            float z = localVelocity.z;

            if (z > 0.5f)
            {
                if (x > 0.5f) return MoveDirection8.ForwardRight;
                if (x < -0.5f) return MoveDirection8.ForwardLeft;
                return MoveDirection8.Forward;
            }

            if (z < -0.5f)
            {
                if (x > 0.5f) return MoveDirection8.BackRight;
                if (x < -0.5f) return MoveDirection8.BackLeft;
                return MoveDirection8.Back;
            }

            if (x > 0.5f) return MoveDirection8.Right;
            if (x < -0.5f) return MoveDirection8.Left;

            return MoveDirection8.Forward;
        }
    }
}