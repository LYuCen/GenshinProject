using System;
using System.Collections.Generic;
using System.IO;
using AssetBundleFramework.Core.Bundle;
using UnityEngine;

namespace AssetBundleFramework.Core.Resource
{
    public class ResourceManager
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static ResourceManager instance { get; } = new ResourceManager();

        private bool m_Initialized;

        private const string MANIFEST_BUNDLE = "manifest.ab";
        private const string RESOURCE_ASSET_NAME = "Assets/Temp/Resource.bytes";
        private const string BUNDLE_ASSET_NAME = "Assets/Temp/Bundle.bytes";
        private const string DEPENDECY_ASSET_NAME = "Assets/Temp/Dependency.bytes";
        
        /// <summary>
        /// 保存  AssetBundle 中所有的 shader 资源
        /// </summary>
        private Dictionary<string, Shader> m_ShaderCache = new Dictionary<string, Shader>();

        /// <summary>
        /// 保存资源对应的 bundle
        /// </summary>
        internal Dictionary<string, string> ResourceBundleDic = new Dictionary<string, string>();
        
        /// <summary>
        /// 保存资源的依赖关系
        /// </summary>
        internal Dictionary<string, List<string>> ResourceDependencyDic = new Dictionary<string, List<string>>();

        /// <summary>
        /// 所有资源集合
        /// </summary>
        private Dictionary<string, AResource> m_ResourceDic = new Dictionary<string, AResource>();

        /// <summary>
        /// 需要释放的资源
        /// </summary>
        private LinkedList<AResource> m_NeedUnloadList = new LinkedList<AResource>();

        /// <summary>
        /// 异步加载集合
        /// </summary>
        private List<AResourceAsync> m_AsyncList = new List<AResourceAsync>();
        
        /// <summary>
        /// 是否使用 AssetDataBase 进行加载
        /// </summary>
        private bool m_Editor;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="platform">平台</param>
        /// <param name="getFileCallback">获取资源真实路径回调</param>
        /// <param name="editor">是否使用 AssetDataBase 加载</param>
        /// <param name="offset">获取 Bundle 的偏移</param>
        public static void EnsureInitialized()
        {
            if (instance.m_Initialized) return;

            string platform;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsServer:
                case RuntimePlatform.WindowsPlayer:
                    platform = "Windows";
                    break;
                case RuntimePlatform.Android:
                    platform = "Android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    platform = "iOS";
                    break;
                default:
                    throw new Exception($"未支持的平台:{Application.platform}");
            }

            var prefixPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../../AssetBundle"))
                .Replace("\\", "/");
            prefixPath += $"/{platform}";

            instance.Initialize(platform, assetUrl => $"{prefixPath}/{assetUrl}", false, 0);
        }

        public void Initialize(string platform, Func<string, string> getFileCallback, bool editor, ulong offset)
        {
            if (m_Initialized) return;
            m_Initialized = true;

            m_Editor = editor;

            if (m_Editor) return;

            BundleManager.instance.Initialize(platform, getFileCallback, offset);

            string manifestBundleFile = getFileCallback.Invoke(MANIFEST_BUNDLE);
            AssetBundle manifestAssetBundle = AssetBundle.LoadFromFile(manifestBundleFile, 0, offset);
            
            TextAsset resourceTextAsset = manifestAssetBundle.LoadAsset(RESOURCE_ASSET_NAME) as TextAsset;
            TextAsset bundleTextAsset = manifestAssetBundle.LoadAsset(BUNDLE_ASSET_NAME) as TextAsset;
            TextAsset dependencyTextAsset = manifestAssetBundle.LoadAsset(DEPENDECY_ASSET_NAME) as TextAsset;

            byte[] resourceBytes = resourceTextAsset.bytes;
            byte[] bundleBytes = bundleTextAsset.bytes;
            byte[] dependencyBytes = dependencyTextAsset.bytes;
            
            manifestAssetBundle.Unload(true); // 拿到 byte 后就可以不用这个了
            manifestAssetBundle = null;

            // 保存 id 对应的 Asseturl
            Dictionary<ushort, string> assetUrlDic = new Dictionary<ushort, string>();
            
            #region 读取资源信息
            {
                MemoryStream resourceMemoryStream = new MemoryStream(resourceBytes);
                BinaryReader resourceBinaryWriter = new BinaryReader(resourceMemoryStream);
                // 获取资源个数
                ushort resourceCount = resourceBinaryWriter.ReadUInt16();
                //Debug.Log($"资源条数{resourceCount}");
                for (ushort i = 0; i < resourceCount; i++)
                {
                    string assetUrl = resourceBinaryWriter.ReadString();
                    assetUrlDic.Add(i, assetUrl);
                }
            }
            #endregion

            #region 读取 bundle 信息
            {
                ResourceBundleDic.Clear();
                MemoryStream bundleMemoryStream = new MemoryStream(bundleBytes);
                BinaryReader bundleBinaryReader = new BinaryReader(bundleMemoryStream);
                
                // 获取 Bundle 个数
                ushort bundleCount = bundleBinaryReader.ReadUInt16();
                for (ushort i = 0; i < bundleCount; i++)
                {
                    string bundleUrl = bundleBinaryReader.ReadString();
                    string bundleFileUrl = bundleUrl;
                    ushort resourceCount = bundleBinaryReader.ReadUInt16();
                    //Debug.Log($"{bundleUrl}中包含{resourceCount}个文件");
                    for (int ii = 0; ii < resourceCount; ii++)
                    {
                        ushort assetId = bundleBinaryReader.ReadUInt16();
                        string assetUrl = assetUrlDic[assetId];
                        ResourceBundleDic.Add(assetUrl, bundleFileUrl);
                    }
                }
            }
            #endregion

            #region 读取资源依赖信息
            {
                ResourceDependencyDic.Clear();
                MemoryStream dependencyMemoryStream = new MemoryStream(dependencyBytes);
                BinaryReader dependencyBinaryReader = new BinaryReader(dependencyMemoryStream);
                
                // 获取依赖个数
                ushort dependencyCount = dependencyBinaryReader.ReadUInt16();
                for (int i = 0; i < dependencyCount; i++)
                {
                    // 获取资源个数
                    ushort resourceCount = dependencyBinaryReader.ReadUInt16();
                    ushort assetId = dependencyBinaryReader.ReadUInt16();
                    string assetUrl = assetUrlDic[assetId];
                    List<string> dependencyList = new List<string>(resourceCount);
                    //Debug.Log($"依赖条目：{assetUrl}中包含{resourceCount}个文件");
                    for (int ii = 1; ii < resourceCount; ii++) // 依赖数组 0 下半保存的是文件地址id，后面下半保存的是依赖文件地址id
                    {
                        ushort dependencyAssetId = dependencyBinaryReader.ReadUInt16();
                        string dependencyUrl = assetUrlDic[dependencyAssetId];
                        dependencyList.Add(dependencyUrl);
                    }

                    ResourceDependencyDic.Add(assetUrl, dependencyList);
                }
            }
            #endregion
            //PreloadShaders();
        }
        
        /// <summary>
        /// 获取 shader 方法
        /// </summary>
        /// <param name="shaderName"></param>
        /// <returns></returns>
        public Shader GetShaderFromCache(string shaderName)
        {
            m_ShaderCache.TryGetValue(shaderName, out var s);
            return s;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="url">资源 url</param>
        /// <param name="async">是否异步</param>
        /// <param name="callback">加载完成回调</param>
        public void LoadWithCallback(string url, bool async, Action<IResource> callback)
        {
            AResource resource = LoadInternal(url, async, false);
            if (resource.done)
            {
                callback?.Invoke(resource);
            }
            else
            {
                resource.finishedCallback += callback;
            }
        }
        
        /// <summary>
        /// 内部加载资源
        /// </summary>
        /// <param name="url">资源 url</param>
        /// <param name="async">是否异步</param>
        /// <param name="dependency">是否依赖</param>
        /// <returns></returns>
        private AResource LoadInternal(string url, bool async, bool dependency)
        {
            AResource resource = null;
            if (m_ResourceDic.TryGetValue(url, out resource))
            {
                if (resource.reference == 0)
                {
                    m_NeedUnloadList.Remove(resource);
                }
                
                resource.AddReference();

                return resource;
            }

            if (m_Editor)
            {
                resource = new EditorResource();
            }
            else if (async)
            {
                ResourceAsync resourceAsync = new ResourceAsync();
                m_AsyncList.Add(resourceAsync);
                resource = resourceAsync;
            }
            else
            {
                resource = new Resource();
            }

            resource.url = url;
            m_ResourceDic.Add(url, resource);

            // 加载依赖
            List<string> dependencies = null;
            ResourceDependencyDic.TryGetValue(url, out dependencies);
            if (dependencies != null && dependencies.Count > 0)
            {
                resource.dependencies = new AResource[dependencies.Count];
                for (int i = 0; i < dependencies.Count; i++)
                {
                    string dependencyUrl = dependencies[i];
                    AResource dependencyResource = LoadInternal(dependencyUrl, async, true);
                    resource.dependencies[i] = dependencyResource;
                }
            }
            
            resource.AddReference();
            resource.Load();
            
            return resource;
        }
        
        

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="resource"></param>
        public void UnLoad(IResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentException(
                    $"{nameof(ResourceManager)}.{nameof(UnLoad)}() {nameof(resource)} is null.");
            }
            
            AResource aResource = resource as AResource;
            aResource.ReduceReference();

            if (aResource.reference == 0)
            {
                WillUnLoad(aResource);
            }
        }

        /// <summary>
        /// 即将要释放的资源
        /// </summary>
        /// <param name="resource">资源路径</param>
        private void WillUnLoad(AResource resource)
        {
            m_NeedUnloadList.AddLast(resource);
        }

        public void Update()
        {
            BundleManager.instance.Update();

            for (int i = 0; i < m_AsyncList.Count; i++)
            {
                AResourceAsync resourceAsync = m_AsyncList[i];
                if (resourceAsync.Update())
                {
                    m_AsyncList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void LateUpdate()
        {
            if (m_NeedUnloadList.Count != 0)
            {
                while (m_NeedUnloadList.Count > 0)
                {
                    AResource resource = m_NeedUnloadList.First.Value;
                    m_NeedUnloadList.RemoveFirst();

                    if (resource == null) continue;

                    m_ResourceDic.Remove(resource.url);

                    resource.UnLoad();

                    if (resource.dependencies != null)
                    {
                        for (int i = 0; i < resource.dependencies.Length; i++)
                        {
                            AResource temp = resource.dependencies[i];
                            UnLoad(temp);
                        }
                    }
                }
            }
            
            BundleManager.instance.LateUpdate();
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="url">资源 url</param>
        /// <param name="async">是否异步</param>
        /// <returns></returns>
        public IResource Load(string url, bool async)
        {
            return LoadInternal(url, async, false);
        }
    }
}