using AssetBundleFramework.Core.Resource;
using BaseClass;
using UnityEngine;

namespace Firearms
{
    public class FireMuzzleObjectPool : AbstractObjPool<FireMuzzleObjectPool>
    {
        protected override GameObject InitPrefab()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/Pyro/muzzle.prefab", false);
            return ir.GetAsset<GameObject>();
        }
    }
}