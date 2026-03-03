using System.Collections.Generic;
using Common.LevelFSM.EnemyFSM.LevelFSMState;
using UnityEngine;

namespace AI
{
    public class EnemyPatrol : MonoBehaviour
    {
        public float waitTime = 1f;
        public Transform patrolLine;
        public List<Transform> points;
        public EnemyAI enemyAI;
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

        private void Start()
        {
            if (!patrolLine || points.Count == 0)
                return;

            enemyAI.MoveToAim(points[0]);
        }

        private void Update()
        {
            if (!patrolLine || points.Count == 0)
                return;

            if (enemyAI.HasArrived())
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
            if (currentIndex + _add >= points.Count) _add = -1;
            else if (currentIndex + _add < 0) _add = 1;

            currentIndex += _add;
            enemyAI.MoveToAim(points[currentIndex]);
        }
    }
}