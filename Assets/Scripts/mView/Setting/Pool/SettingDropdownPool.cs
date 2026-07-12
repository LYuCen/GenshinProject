using AssetBundleFramework.Core.Resource;
using BaseClass;
using UnityEngine;

namespace mView.Setting.Pool
{
    public class SettingDropdownPool : AbstractObjPool<SettingDropdownPool>
    {
        protected override GameObject InitPrefab()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/UI/Setting/DropdownPrefab.prefab", false);
            return ir.GetAsset<GameObject>();
        }
        
        public override void RemoveObj(GameObject obj) 
        {
            if (IsOnDestroy) return;

            if (Pool.Count < MaxSize)
            {
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.localScale = Vector3.one;
                rt.rotation = Quaternion.identity;     
                Pool.Enqueue(obj);
                obj.gameObject.SetActive(false);       
                obj.transform.SetParent(transform, false);    
            }
            else Destroy(obj);
        }

    }
}