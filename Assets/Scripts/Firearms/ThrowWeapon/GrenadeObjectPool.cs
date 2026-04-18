using AssetBundleFramework.Core.Resource;
using BaseClass;
using UnityEngine;

namespace Firearms.ThrowWeapon
{
    public class GrenadeObjectPool : AbstractObjPool<GrenadeObjectPool>
    {
        protected override GameObject InitPrefab()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/Grenade/Grenade.prefab", false);
            return ir.GetAsset<GameObject>();
        }
    }
}