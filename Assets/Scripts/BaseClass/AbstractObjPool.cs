using System.Collections;
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
        protected bool IsBuild;
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="value">预加载元素数量</param>
        /// <param name="maxSize">对象池大小</param>
        public virtual void Init(int value = 0,int maxSize = 100)
        {
            if (IsBuild) return;
            IsOnDestroy = false;
            Pool = new Queue<GameObject>();
            MaxSize = maxSize;
            Prefab = InitPrefab();
            InitObject(value);
            IsBuild = true;
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
            if (!IsBuild || Prefab == null)
            {
                IsBuild = false;
                Init();
            }

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
                obj.transform.SetParent(transform, false);
            }
            else Destroy(obj);
        }

        public virtual void Clear()
        {
            if (Pool == null) return; 
            while (Pool.Count > 0)
            {
                var obj = Pool.Dequeue();
                if (obj != null) Destroy(obj);
            }
        }

        protected override void OnDestroy()
        {
            IsOnDestroy = true;
            Clear();
        }
    }
}