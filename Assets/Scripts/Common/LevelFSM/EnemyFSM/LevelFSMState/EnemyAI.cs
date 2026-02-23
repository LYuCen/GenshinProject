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
        public void MoveToAim(Transform aim, bool stopPrevAgent = false)
        {
            if (!aim) return;
            if (stopPrevAgent || !navMeshAgent.hasPath) // 强制打断 或 不存在有效路径
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(aim.position);
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
        
        /// <summary>
        /// 沿着玩家随机走位
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="moveDistance">位移距离</param>
        public void StrafeAroundTarget(Transform target, float moveDistance)
        {
            if (!navMeshAgent || !target) return;

            // 左 or 右
            int dir = Random.value > 0.5f ? 1 : -1;
            this.moveOri = dir;

            Vector3 right = Vector3.Cross(Vector3.up, (target.position - transform.position).normalized);
            Vector3 moveDir = right * dir;

            Vector3 targetPos = transform.position + moveDir * moveDistance;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, moveDistance, NavMesh.AllAreas))
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(hit.position);
            }
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
    }
}