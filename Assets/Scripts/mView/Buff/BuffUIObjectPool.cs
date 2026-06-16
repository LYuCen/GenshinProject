using AssetBundleFramework.Core.Resource;
using BaseClass;
using UnityEngine;

namespace mView.Buff
{
    public class BuffUIObjectPool : AbstractObjPool<BuffUIObjectPool>
    {
        protected override GameObject InitPrefab()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/UI/BuffUI.prefab", false);
            return ir.GetAsset<GameObject>();
        }
        
        public override void RemoveObj(GameObject obj)
        {
            if (IsOnDestroy) return;
            
            if (Pool.Count < MaxSize)
            {
                Pool.Enqueue(obj);
                obj.gameObject.SetActive(false);
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.rotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                obj.transform.SetParent(transform);
            }
            else Destroy(obj);
        }
    }
}