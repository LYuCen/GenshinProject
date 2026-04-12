using AssetBundleFramework.Core.Resource;
using BaseClass;
using UnityEngine;

namespace Enemy
{
    public class DieParticleObjectPool : AbstractObjPool<DieParticleObjectPool>
    {
        protected override GameObject InitPrefab()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/Particle/DieParticle.prefab", false);
            return ir.GetAsset<GameObject>();
        }
    }
}