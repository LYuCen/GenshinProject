using System.Collections.Generic;
using GameFSM.LevelFSM.EnemyFSM.LevelFSMState;
using UnityEngine;

namespace AI
{
    public class EnemyPatrol : MonoBehaviour
    {
        public float waitTime = 1f;
        public Transform patrolLine;
        public List<Transform> points;
        public EnemyAI enemyAI;
        public bool enable;
        private int currentIndex;
        private float waitTimer;

        private void Awake()
        {
            points = new List<Transform>();
            foreach (Transform child in patrolLine)
            {
                points.Add(child);
            }
        }

        private void Update()
        {
            if (!patrolLine || points.Count == 0)
                return;

            if (enemyAI.HasArrived() && enable)
            {
                waitTimer += Time.deltaTime;

                if (waitTimer >= waitTime)
                {
                    GoNext();
                    waitTimer = 0;
                }
            }
        }

        private int _add = 1;
        
        void GoNext()
        {
            enemyAI.MoveToAim(points[currentIndex].position);
            
            if (currentIndex + _add >= points.Count) _add = -1;
            else if (currentIndex + _add < 0) _add = 1;
            currentIndex += _add;
            
        }
    }
}