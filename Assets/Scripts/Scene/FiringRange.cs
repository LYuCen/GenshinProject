using System.Collections;
using System.Collections.Generic;
using GameFSM.FSM.PlayerTargetFSM;
using mEvent.FiringRange;
using QFramework;
using ShootingTarget;
using UnityEngine;

namespace Scene
{
    public class FiringRange : MonoBehaviour, IController
    {
        [Header("网格设置")]
        [SerializeField] private int gridWidth = 10;
        [SerializeField] private int gridHeight = 10;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.3f);

        [Header("标靶设置")]
        [SerializeField] private int targetCount = 1;
        [SerializeField] private float respawnDelay = 2f;

        private readonly int[] _laneFlags = new int[10];
        private readonly Dictionary<PlayerTarget, int> _laneMap = new Dictionary<PlayerTarget, int>();

        private bool _toggleMove;
        private bool _toggleCrouch;

        private void Start()
        {
            this.GetArchitecture();
            PlayerTargetObjPool.Instance.Init();

            for (int i = 0; i < targetCount; i++)
                InitPlayerTarget();
            
            this.RegisterEvent<TogglePlayerTargetMoveEvent>(_ =>
            {
                _toggleMove = !_toggleMove;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<TogglePlayerTargetCrouchEvent>(_ =>
            {
                _toggleCrouch = !_toggleCrouch;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<AddPlayerTargetValueEvent>(e =>
            {
                if (targetCount + e.Value >= 1 && targetCount + e.Value <= 10)
                {
                    targetCount += e.Value;
                    if (e.Value > 0)
                    {
                        InitPlayerTarget();
                    }
                    else
                    {
                        int index = -1;
                        for (int i = _laneFlags.Length - 1; i >= 0; i--)
                        {
                            if (_laneFlags[i] != 0)
                            {
                                index = i;
                                break;
                            }
                        }

                        PlayerTarget target = null;
                        foreach (var kv in _laneMap)
                        {
                            if (kv.Value == index)
                            {
                                target = kv.Key;

                                break;
                            }
                        }

                        if (target)
                        {
                            target.Dead -= TargetDead;
                            _laneMap.Remove(target);
                            PlayerTargetObjPool.Instance.RemoveObj(target.gameObject);
                            _laneFlags[index] = 0;
                        }
                    }
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void InitPlayerTarget()
        {
            for (int i = 0; i < _laneFlags.Length; i++)
            {
                if (_laneFlags[i] == 0)
                {
                    GameObject obj = PlayerTargetObjPool.Instance.GetObj();
                    Vector3 pos = transform.position;
                    pos.z += i * cellSize;
                    pos.x += Random.Range(-gridWidth / 2, gridWidth / 2) * cellSize;
                    obj.transform.SetPositionAndRotation(pos, Quaternion.Euler(0f, 180f, 0f));

                    _laneFlags[i] = 1;
                    var target = obj.GetComponent<PlayerTarget>();
                    if (target)
                    {
                        _laneMap[target] = i;
                        target.Dead += TargetDead;
                    }

                    var ai = obj.GetComponent<PlayerTargetAI>();
                    ai.Init(_toggleMove, _toggleCrouch);
                    return;
                }
            }
        }

        private void TargetDead(PlayerTarget target)
        {
            target.Dead -= TargetDead;
            StartCoroutine(RespawnAfterDelay(target));
        }

        private IEnumerator RespawnAfterDelay(PlayerTarget target)
        {
            yield return new WaitForSeconds(respawnDelay);

            target.Reset();

            if (_laneMap.TryGetValue(target, out int lane))
            {
                Vector3 pos = transform.position;
                pos.z += lane * cellSize;
                pos.x += Random.Range(-gridWidth / 2, gridWidth / 2) * cellSize;
                target.transform.SetPositionAndRotation(pos, Quaternion.Euler(0f, 180f, 0f));
            }

            target.Dead += TargetDead;
        }

        private void OnDrawGizmos()
        {
            float totalW = gridWidth * cellSize;
            float totalH = gridHeight * cellSize;
            Vector3 origin = transform.position - new Vector3(totalW * 0.5f, 0f, totalH * 0.5f);

            for (int x = 0; x <= gridWidth; x++)
            {
                Vector3 start = origin + new Vector3(x * cellSize, 0f, 0f);
                Vector3 end = start + new Vector3(0f, 0f, totalH);
                Gizmos.color = gizmoColor;
                Gizmos.DrawLine(start, end);
            }

            for (int z = 0; z <= gridHeight; z++)
            {
                Vector3 start = origin + new Vector3(0f, 0f, z * cellSize);
                Vector3 end = start + new Vector3(totalW, 0f, 0f);
                Gizmos.color = gizmoColor;
                Gizmos.DrawLine(start, end);
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}
