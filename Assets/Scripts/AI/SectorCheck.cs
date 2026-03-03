using UnityEngine;

namespace AI
{
    /// <summary>
    /// 扇形区域判定类
    /// </summary>
    public class SectorChecker
    {
        
        public float Radius;
        public float Angle;
        public bool CheckObstacle = false; // 遮挡检测
        public LayerMask ObstacleLayer; // 遮挡层

        /// <summary>
        /// 判断目标点是否在扇形区域内
        /// </summary>
        /// <param name="origin">扇形中心</param>
        /// <param name="forward">扇形前方方向（单位向量）</param>
        /// <param name="target">目标点位置</param>
        /// <returns>目标是否在扇形内</returns>
        public bool IsInside(Vector3 origin, Vector3 forward, Vector3 target)
        {
            Vector3 dir = target - origin;

            // 超出半径
            float sqrDist = dir.sqrMagnitude;
            if (sqrDist > Radius * Radius)
                return false;

            // 扇形角度判定
            float halfAngle = Angle * 0.5f;
            float dot = Vector3.Dot(forward.normalized, dir.normalized);
            float angleToTarget = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (angleToTarget > halfAngle)
                return false;

            // 遮挡检测
            if (CheckObstacle)
            {
                if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, Radius, ObstacleLayer))
                {
                    // 如果射线击中了目标之外的障碍物
                    if (hit.collider.transform.position != target)
                        return false;
                }
            }

            // 通过所有判定
            return true;
        }
    }
}