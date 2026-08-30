using AssetBundleFramework.Core.Resource;
using BaseClass;
using UnityEngine;

namespace mView.Room
{
    public class SceneItemObjPool : AbstractObjPool<SceneItemObjPool>
    {
        protected override GameObject InitPrefab()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/UI/Room/SceneItem.prefab", false);
            return ir.GetAsset<GameObject>();
        }
    }
}