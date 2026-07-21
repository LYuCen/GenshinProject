using AssetBundleFramework.Core.Resource;
using BaseClass;
using UnityEngine;

namespace Scene
{
    public class PlayerTargetObjPool : AbstractObjPool<PlayerTargetObjPool>
    {
        protected override GameObject InitPrefab()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/Player/PlayerTarget.prefab", false);
            return ir.GetAsset<GameObject>();
        }
    }
}