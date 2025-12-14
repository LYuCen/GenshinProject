using System.Collections.Generic;
using UnityEngine;

namespace BaseClass
{
    public abstract class AbstractObjPool<T> : AbstractSingleton<T> where T : MonoBehaviour
    {
        protected int MaxSize;
        protected Queue<GameObject> Pool;
        protected GameObject Prefab;
        protected bool IsOnDestroy;
        
        public void Init(GameObject prefab, int maxSize = 100)
        {
            IsOnDestroy = false;
            Pool = new Queue<GameObject>();
            Prefab = prefab;
            MaxSize = maxSize;
            RemoveObj(Instantiate(Prefab));
        }

        public virtual GameObject GetObj()
        {
            GameObject res;
            if (Pool.Count > 0)
            {
                res = Pool.Dequeue();
            }
            else res = Instantiate(Prefab);
            res.SetActive(true);
            return res;
        }
        public virtual void RemoveObj(GameObject obj)
        {
            if (IsOnDestroy) return;
            
            if (Pool.Count < MaxSize)
            {
                Pool.Enqueue(obj);
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(transform);
            }
            else Destroy(obj);
        }

        private void OnDestroy()
        {
            IsOnDestroy = true;
            while (Pool != null && Pool.Count > 0)
            {
                var item = Pool.Dequeue();
                Destroy(item);
            }
        }
    }
}