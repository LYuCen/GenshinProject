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
        private bool _isBuild;
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="value">预加载元素数量</param>
        /// <param name="maxSize">对象池大小</param>
        public virtual void Init(int value = 0,int maxSize = 100)
        {
            if (_isBuild) return;
            IsOnDestroy = false;
            Pool = new Queue<GameObject>();
            MaxSize = maxSize;
            Prefab = InitPrefab();
            InitObject(value);
            _isBuild = true;
        }

        protected void InitObject(int val)
        {
            GameObject[] objs = new GameObject[val];
            for (int i = 0; i < val; i++)
            {
                objs[i] = GetObj();
            }

            for (int i = 0; i < val; i++)
            {
                RemoveObj(objs[i]);
            }
        }

        protected abstract GameObject InitPrefab();

        public virtual GameObject GetObj()
        {
            if (!_isBuild) Init();
            if (!Prefab) return null;
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