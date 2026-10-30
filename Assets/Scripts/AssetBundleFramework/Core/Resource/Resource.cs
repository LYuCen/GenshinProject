using System;
using AssetBundleFramework.Core.Bundle;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetBundleFramework.Core.Resource
{
    internal class Resource : AResource
    {
        public override bool keepWaiting => !done;
        internal override void Load()
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException($"{nameof(Resource)}.{nameof(Load)}() {nameof(url)} is null.");
            }

            if (bundle != null)
            {
                throw new Exception($"{nameof(Resource)}.{nameof(Load)}() {nameof(bundle)} not null.");
            }

            string bundleUrl = null;
            if (!ResourceManager.instance.ResourceBundleDic.TryGetValue(url, out bundleUrl))
            {
                throw new Exception($"{nameof(Resource)}.{nameof(Load)}() {nameof(bundleUrl)} is null");
            }

            bundle = BundleManager.instance.Load(bundleUrl);
            LoadAsset();
        }

        internal override void UnLoad()
        {
            if (bundle == null)
            {
                throw new Exception($"{nameof(Resource)}.{nameof(UnLoad)}() {nameof(bundle)} is null.");
            }

            if (asset != null && !(asset is GameObject))
            {
                Resources.UnloadAsset(asset);
                asset = null;
            }
            
            BundleManager.instance.UnLoad(bundle);
            bundle = null;
            finishedCallback = null;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        internal override void LoadAsset()
        {
            if (bundle == null)
            {
                throw new Exception($"{nameof(Resource)}.{nameof(LoadAsset)}() {nameof(bundle)} is null.");
            }
            
            // 正在异步加载的资源要变成同步
            FreshAsyncAsset();

            if (!bundle.isStreamedSceneAssetBundle)
            {
                asset = bundle.LoadAsset(url, typeof(Object));
            }

            // 注意：你原来的代码这里写了两遍 LoadAsset，我合并并优化了逻辑
            if (asset == null)
            {
                asset = bundle.LoadAsset(url, typeof(Object));
            }

            #if UNITY_EDITOR
            if (asset is GameObject go)
            {
                RuntimeFixShaders(go);
            }
            #endif
            
            
            done = true;

            if (finishedCallback != null)
            {
                Action<AResource> tempCallback = finishedCallback;
                finishedCallback = null;
                tempCallback.Invoke(this);
            }
        }
        
        /// <summary>
        /// 在运行环境下，AssetBundle 加载的 Shader 需要手动刷新关联
        /// </summary>
        private void RuntimeFixShaders(GameObject go)
        {
            var renderers = go.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                // 关键：这里要处理所有的材质
                Material[] materials = renderer.sharedMaterials;
                bool changed = false;

                for (int i = 0; i < materials.Length; i++)
                {
                    Material mat = materials[i];
                    if (mat == null) continue;

                    // 获取当前材质引用的 Shader 名称
                    string shaderName = mat.shader.name;
            
                    // 从我们的缓存字典里找 Shader 对象
                    Shader targetShader = ResourceManager.instance.GetShaderFromCache(shaderName);

                    // 如果缓存里有，且跟当前材质引用的不是同一个对象
                    if (targetShader != null && mat.shader != targetShader)
                    {
                        mat.shader = targetShader;
                        changed = true;
                    }
                    Debug.Log($"尝试修复材质:{mat.name} 原始Shader:{mat.shader.name} 目标Shader:{targetShader?.name}");
                    Debug.Log($"Shader:{mat.shader.name} 是否支持当前平台:{mat.shader.isSupported}");
                }
        
                // 如果修改了材质，有些情况下需要重新赋值回 renderer (虽然 sharedMaterials 通常是引用的)
                if (changed) {
                    renderer.sharedMaterials = materials;
                }
            }
        }

        public override T GetAsset<T>()
        {
            Object tempAsset = asset;
            Type type = typeof(T);

            if (type == typeof(Sprite))
            {
                if (asset is Sprite) return tempAsset as T;
                else
                {
                    if (tempAsset && !(tempAsset is GameObject))
                    {
                        Resources.UnloadAsset(tempAsset);
                    }

                    asset = bundle.LoadAsset(url, type);
                    return asset as T;
                }
            }
            return tempAsset as T;
        }
    }
}