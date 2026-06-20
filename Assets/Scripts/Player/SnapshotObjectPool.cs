using BaseClass;
using Enemy;
using UnityEngine;

namespace Player
{
    public class SnapshotObjectPool : AbstractObjPool<SnapshotObjectPool>
    {
        private const int MaxMeshParts = 10;
        
        protected override GameObject InitPrefab()
        {
            GameObject root = new GameObject("PooledSnapshot");
            for (int i = 0; i < MaxMeshParts; i++)
            {
                GameObject child = new GameObject($"Part_{i}");
                child.transform.SetParent(root.transform);
                child.AddComponent<MeshFilter>();
                child.AddComponent<MeshRenderer>();
                child.SetActive(false);
            }

            root.AddComponent<ShowDieParticle>();
            return root;
        }

        public override GameObject GetObj()
        {
            GameObject obj = base.GetObj();
            var sdp = obj.GetComponent<ShowDieParticle>();
            sdp.ResetState();
            sdp.Action = _ =>
            {
                GameObject particle = DieParticleObjectPool.Instance.GetObj();
                particle.transform.position = obj.transform.position + Vector3.up;
                particle.SetActive(true);
                Debug.Log("放回对象池中");
                Instance.RemoveObj(obj);
            };
            return obj;
        }
    }
}