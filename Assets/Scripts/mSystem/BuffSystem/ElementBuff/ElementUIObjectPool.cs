using AssetBundleFramework.Core.Resource;
using BaseClass;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff
{
    public class ElementUIObjectPool : AbstractObjPool<ElementUIObjectPool>
    {
        protected override GameObject InitPrefab()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Model/ElementUI.prefab", false);
            return ir.GetAsset<GameObject>();
        }
    }
}