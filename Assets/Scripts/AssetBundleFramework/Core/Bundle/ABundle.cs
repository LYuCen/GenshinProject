using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetBundleFramework.Core.Bundle
{
    internal abstract class ABundle
    {
        internal AssetBundle assetBundle { get; set; }
        
        /// <summary>
        /// Bundle url
        /// </summary>
        internal string url { get; set; }
        
        /// <summary>
        /// 引用计数器
        /// </summary>
        internal int reference { get; set; }
        
        // bundle 是否加载完成
        internal bool done { get; set; }
        
        /// <summary>
        /// bundle 依赖
        /// </summary>
        internal ABundle[] dependencies { get; set; }

        /// <summary>
        /// 是否是场景
        /// </summary>
        internal bool isStreamedSceneAssetBundle { get; set; }

        /// <summary>
        /// 加载 bundle
        /// </summary>
        internal abstract void Load();

        /// <summary>
        /// 卸载 bundle
        /// </summary>
        internal abstract void UnLoad();

        internal void AddReference()
        {
            // 自己引用 +1
            ++reference;
        }

        /// <summary>
        /// 减少引用
        /// </summary>
        internal void ReduceReference()
        {
            --reference;
            if (reference < 0)
            {
                throw new Exception($"{GetType()}.{nameof(ReduceReference)}() less than 0.{nameof(url)}:{url}.");
            }
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="type">资源 Type</param>
        /// <returns>指定名字的资源</returns>
        internal abstract Object LoadAsset(string name, Type type);

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="type">资源 type</param>
        /// <returns>AssetBundleRequest</returns>
        internal abstract AssetBundleRequest LoadAssetAsync(string name, Type type);
    }
}