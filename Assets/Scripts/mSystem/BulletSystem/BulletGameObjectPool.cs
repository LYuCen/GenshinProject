using AssetBundleFramework.Core.Resource;
using BaseClass;
using UnityEngine;

namespace mSystem.BulletSystem
{
    public class BulletGameObjectPool : AbstractObjPool<BulletGameObjectPool>
    {
        protected override GameObject InitPrefab()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/Bullet/Bullet.prefab", false);
            return ir.GetAsset<GameObject>();
        }
    }
}