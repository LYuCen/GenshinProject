using UnityEngine;
using UnityEngine.AI;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private EnemyManager manager;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Transform playerTrans;
        [Header("移动朝向")] [SerializeField] private Vector3 front;
        [Header("旋转速度")] [SerializeField] private float angularSpeed;
        [Header("检测半径")] public float detectRadius = 5f;
        [Header("追踪半径")] public float traceRadius = 15f;
        public LayerMask playerLayer;
        private Collider[] _hits = new Collider[4];
        [SerializeField] private bool isStopping;

        void Update()
        {
            manager.moveSpeed = navMeshAgent.velocity.magnitude;
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                detectRadius,
                _hits,
                playerLayer
            );
            
            if (count > 0)
            {
                playerTrans = _hits[0].transform.root;
                front = playerTrans.position - transform.position;
                front.y = 0;

                float distance = front.magnitude; // 与玩家距离
                
                isStopping = !navMeshAgent.pathPending  // 不在寻路
                             && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance // 在目标范围
                             && navMeshAgent.velocity.sqrMagnitude < 0.01f;    // 速度几乎为 0
                
                // 如果寻路结束，下次启动则在角色离开 traceRadius 距离后启动
                if(!isStopping || distance >= traceRadius) navMeshAgent.SetDestination(playerTrans.position);
                
                if (front.sqrMagnitude > 0.001f) // 避免零向量报错
                {
                    front.Normalize();

                    // 当前朝向
                    Quaternion currentRot = transform.rotation;

                    // 目标朝向
                    Quaternion targetRot = Quaternion.LookRotation(front);

                    // 平滑旋转，参数：最大角速度 * deltaTime
                    transform.rotation = Quaternion.RotateTowards(
                        currentRot,
                        targetRot,
                        angularSpeed * Time.deltaTime
                    );
                }
                
            }
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }
    }
}