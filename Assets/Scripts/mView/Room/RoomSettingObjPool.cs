using AssetBundleFramework.Core.Resource;
using BaseClass;
using UnityEngine;

namespace mView.Room
{
    public class RoomSettingObjPool : AbstractObjPool<RoomSettingObjPool>
    {
        protected override GameObject InitPrefab()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/UI/Room/RoomSettingItem.prefab", false);
            return ir.GetAsset<GameObject>();
        }
        
        public override GameObject GetObj()
        {
            if(!IsBuild) Init();
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
    }
}