using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace mSystem.AISystem
{
    public interface IAISystem
    {
        /// <summary>
        /// 获取掩体信息
        /// </summary>
        /// <param name="self">敌人位置</param>
        /// <param name="findRadius">获取半径</param>
        /// <returns></returns>
        public CoverPoint[] FindCoverPoints(Transform self, float findRadius);

        public void SetTarget(Transform target);
    }

    public class AISystem : IAISystem
    {
        private Transform _target;

        public AISystem(Transform playerTransform)
        {
            _target = playerTransform;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }
        
        public CoverPoint[] FindCoverPoints(Transform self, float findRadius)
        {
            Collider[] colliders = Physics.OverlapSphere(self.position, findRadius, LayerMask.GetMask("Cover"));

            List<CoverPoint> validCovers = new List<CoverPoint>();

            foreach (var col in colliders)
            {
                Transform cover = col.transform;

                if (!IsCoverValid(self, cover))
                    continue;
                
                validCovers.Add(cover.GetComponent<CoverPoint>());
            }

            return validCovers.ToArray();
        }
        
        private bool IsCoverValid(Transform self, Transform cover)
        {
            if (_target == null) return false;
            
            /*
            // 1. 是否能挡住目标
            Vector3 dir = _target.position - self.position;

            if (!Physics.Raycast(self.position, dir, out RaycastHit hit))
                return false;

            if (hit.transform != cover)
                return false;
            */

            // 2. 是否可达
            NavMeshPath path = new NavMeshPath();
            if (!NavMesh.CalculatePath(self.position, cover.position, NavMesh.AllAreas, path))
                return false;

            return path.status == NavMeshPathStatus.PathComplete;
        }
    }
}