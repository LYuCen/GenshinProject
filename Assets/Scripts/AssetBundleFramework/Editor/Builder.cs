using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AssetBundleFramework.Core;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace AssetBundleFramework.Editor
{
    public static class Builder
    {
        public static readonly Vector2 collectRuleFileProgress = new Vector2(0, 0.2f);
        public static readonly Vector2 ms_GetDependencyProgress = new Vector2(0.2f, 0.4f);
        public static readonly Vector2 ms_CollectBundleInfoProgress = new Vector2(0.4f, 0.5f);
        public static readonly Vector2 ms_GenerateBuildInfoProgress = new Vector2(0.5f, 0.6f);
        public static readonly Vector2 ms_BuildBundleProgress = new Vector2(0.6f, 0.7f);
        public static readonly Vector2 ms_ClearBundleProgress = new Vector2(0.7f, 0.9f);
        public static readonly Vector2 ms_BuildManifestProgress = new Vector2(0.9f, 1.0f);
        
        private static readonly Profiler ms_buildProfiler = new Profiler(nameof(Builder));
        private static readonly Profiler ms_loadBuildSettingProfiler = ms_buildProfiler.CreateChild(nameof(SwitchPlatform));
        private static readonly Profiler ms_switchPlatformProfiler = ms_buildProfiler.CreateChild(nameof(LoadSetting));
        private static readonly Profiler ms_collectProfiler = ms_buildProfiler.CreateChild(nameof(LoadSetting));
        private static readonly Profiler ms_collectBuildSettingFile = ms_collectProfiler.CreateChild("CollectBuildSettingFile");
        private static readonly Profiler ms_collectDependencyProfiler =
            ms_collectProfiler.CreateChild(nameof(CollectDependency));

        private static readonly Profiler ms_collectBundleProfiler = ms_collectProfiler.CreateChild(nameof(CollectBundle));

        private static readonly Profiler ms_GenerateManifestPofiler =
            ms_collectProfiler.CreateChild(nameof(GenerateManifest));

        private static readonly Profiler ms_BuildBundleProfiler = ms_buildProfiler.CreateChild(nameof(BuildBundle));
        private static readonly Profiler ms_ClearBundleProfiler = ms_buildProfiler.CreateChild(nameof(ClearAssetBundle));
        private static readonly Profiler ms_BuildManifestBundleProfiler = ms_buildProfiler.CreateChild(nameof(BuildManifest));

#if UNITY_IOS
    private const string PLATFORM = "ios";
#elif UNITY_ANDROID
    private const string PLATFORM = "android"
#else
    private const string PLATFORM = "windows";
#endif
        
        // bundle 后缀。 manifest 发布 AssetBundle 会被打包，这样就无法进行热更新了 所以要剔除
        public const string BUNDLE_SUFFIX = ".ab";
        public const string BUNDLE_MANIFEST_SUFFIX = ".manifest";
        
        // bundle 描述文件
        public const string MANIFEST = "manifest";

        public static readonly ParallelOptions ParallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2
        };
        
        
        // bundle 打包的 Options
        public readonly static BuildAssetBundleOptions BuildAssetBundleOptions =
            BuildAssetBundleOptions.ChunkBasedCompression |
            BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.StrictMode |
            BuildAssetBundleOptions.DisableLoadAssetByFileName |
            BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;
        
        
        /// <summary>
        /// 打包设置
        /// </summary>
        public static BuildSetting BuildSetting { get; private set; }
        
        /// <summary>
        /// 打包目录
        /// </summary>
        public static string buildPath { get; set; }

        /// <summary>
        /// 临时目录，临时生成的文件都统一在该目录
        /// </summary>
        public readonly static string tempPath = Path.GetFullPath(Path.Combine(Application.dataPath, "Temp")).Replace("\\", "/");

        /// <summary>
        /// 资源描述文本
        /// </summary>
        public readonly static string resourcePath_Text = $"{tempPath}/Resource.txt";
        
        /// <summary>
        /// 资源描述二进制
        /// </summary>
        public readonly static string resourcePath_Binary = $"{tempPath}/Resource.bytes";
        
        /// <summary>
        /// bundle 描述文本
        /// </summary>
        public readonly static string bundlePath_Text = $"{tempPath}/Bundle.txt";
        
        /// <summary>
        /// bundle 描述二进制
        /// </summary>
        public readonly static string bundlePath_Binary = $"{tempPath}/Bundle.bytes";
        
        /// <summary>
        /// 资源依赖描述文本
        /// </summary>
        public readonly static string dependencyPath_Text = $"{tempPath}/Dependency.txt";
        
        /// <summary>
        /// 资源依赖描述二进制
        /// </summary>
        public readonly static string dependencyPath_Binary = $"{tempPath}/Dependency.bytes";

        /// <summary>
        /// 临时目录，临时文件的 ab 包都放在该文件夹，打包完成后会移出
        /// </summary>
        public readonly static string TempBuildPath =
            Path.GetFullPath(Path.Combine(Application.dataPath, "../TempBuild")).Replace("\\", "/");
        
        /// <summary>
        /// 打包配置
        /// </summary>
        public readonly static string BuildSettingPath = Path.GetFullPath("BuildSetting.xml").Replace("\\", "/");
        
        #region Build MenuItem

        [MenuItem(("Tools/ResBuild/Windows"))]
        public static void BuildWindows()
        {
            Debug.Log("执行了BuildWindows");
            Build();
        }

        public static void SwitchPlatform()
        {
            string platform = PLATFORM;

            switch (platform)
            {
                case "windows":
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone,
                        BuildTarget.StandaloneWindows64);
                    break;
                case "android":
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,
                        BuildTarget.Android);
                    break;
                case "ios":
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS,
                        BuildTarget.iOS);
                    break;
            }
        }

        private static BuildSetting LoadSetting(string settingPath)
        {
            BuildSetting = XmlUtility.Read<BuildSetting>(settingPath);
            if (BuildSetting == null)
            {
                throw new Exception($"Load buildSetting failed, SettingPath:{settingPath}");
            }
            
            (BuildSetting as ISupportInitialize)?.EndInit();

            buildPath = Path.GetFullPath(BuildSetting.buildRoot).Replace("\\", "/");
            if (buildPath.Length > 0 && buildPath[buildPath.Length - 1] != '/')
            {
                buildPath += "/";
            }

            buildPath += $"{PLATFORM}/";
            Debug.Log($"打包目录:{buildPath}");
            
            return BuildSetting;
        }

        private static void Build()
        {
            ms_buildProfiler.Start();
            
            ms_switchPlatformProfiler.Start();
            SwitchPlatform();
            ms_switchPlatformProfiler.Stop();
            
            ms_loadBuildSettingProfiler.Start();
            BuildSetting = LoadSetting(BuildSettingPath);
            ms_loadBuildSettingProfiler.Stop();
            
            // 收集 bundle 信息
            ms_collectProfiler.Start();
            Dictionary<string, List<string>> bundleDic = Collect();
            ms_collectProfiler.Stop();
            
            // 打包 AssetBundle
            ms_BuildBundleProfiler.Start();
            BuildBundle(bundleDic);
            ms_BuildBundleProfiler.Stop();
            
            // 清理多余文件
            ms_ClearBundleProfiler.Start();
            ClearAssetBundle(buildPath, bundleDic);
            ms_ClearBundleProfiler.Stop();

            // 描述文件打包 bundle
            ms_BuildManifestBundleProfiler.Start();
            BuildManifest();
            ms_BuildManifestBundleProfiler.Stop();
            
            EditorUtility.ClearProgressBar();
            ms_buildProfiler.Stop();

            Debug.Log($"打包完成：{ms_buildProfiler}");
        }
        
        /// <summary>
        /// 外部依赖检查
        /// </summary>
        /// <param name="buildSetting"></param>
        /// <param name="directAssets"></param>
        private static void CheckExternalDependencies(
            BuildSetting buildSetting,
            IEnumerable<string> directAssets)
        {
            HashSet<string> reported = new HashSet<string>();

            foreach (var assetPath in directAssets)
            {
                string[] deps = AssetDatabase.GetDependencies(assetPath, true);

                foreach (var dep in deps)
                {
                    if (dep == assetPath)
                        continue;

                    if (IsAllowedExternalDependency(dep))
                        continue;

                    // 核心判断：是否能被打包规则识别
                    string bundleName = buildSetting.GetBundleName(dep, EResourceType.Dependency);

                    if (bundleName != null)
                        continue;

                    if (reported.Add(dep))
                    {
                        string detailInfo = FindSpecificReferencer(assetPath, dep);
                        Debug.LogWarning(
                            $"[AssetBundle 外部依赖]\n" +
                            $"资源: {assetPath}\n" +
                            $"依赖: {dep}\n" +
                            $"具体引用位置: \n{detailInfo}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 查找资源中具体是哪个对象引用了目标依赖（支持追踪间接依赖链）
        /// </summary>
        private static string FindSpecificReferencer(string assetPath, string targetDepPath)
        {
            // 1. 先尝试在当前文件中直接查找S
            string directResults = SearchInsideAsset(assetPath, targetDepPath);
            if (!string.IsNullOrEmpty(directResults))
            {
                return directResults;
            }

            // 2. 如果当前文件没直接引用它，那一定是“间接引用”
            // 获取当前文件的【直接依赖】（不递归）
            string[] directDeps = AssetDatabase.GetDependencies(assetPath, false);
            List<string> chainInfo = new List<string>();

            foreach (var midDep in directDeps)
            {
                if (midDep == assetPath || midDep == targetDepPath) continue;

                // 检查这个“中间人”是否在它的递归依赖里包含目标非法资源
                string[] midAllDeps = AssetDatabase.GetDependencies(midDep, true);
                bool leadsToTarget = false;
                foreach (var d in midAllDeps)
                {
                    if (d == targetDepPath)
                    {
                        leadsToTarget = true;
                        break;
                    }
                }

                if (leadsToTarget)
                {
                    // 找到了中间人！
                    string reasonInMid = SearchInsideAsset(midDep, targetDepPath);
                    chainInfo.Add($"- [间接引用链]: {assetPath} \n" +
                                  $"    -> 引用了中间资源: {midDep}\n" +
                                  $"    -> 该中间资源引用了非法目标: {targetDepPath}\n" +
                                  $"    -> 引用原因: {(string.IsNullOrEmpty(reasonInMid) ? "隐藏引用(可能是Shader/动画状态机等)" : reasonInMid)}");
                }
            }

            return chainInfo.Count > 0 ? string.Join("\n", chainInfo) : "未找到具体属性引用（可能是Shader内置关联或引擎底层强制引用）";
        }

        /// <summary>
        /// 在单个资源文件内部搜索对目标路径的引用
        /// </summary>
        private static string SearchInsideAsset(string assetPath, string targetDepPath)
        {
            Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            List<string> results = new List<string>();

            foreach (Object obj in allObjects)
            {
                if (obj == null) continue;
                SerializedObject so = new SerializedObject(obj);
                SerializedProperty sp = so.GetIterator();

                while (sp.Next(true)) // 使用 Next(true) 遍历所有属性，包括隐藏的
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference && sp.objectReferenceValue != null)
                    {
                        string refPath = AssetDatabase.GetAssetPath(sp.objectReferenceValue);
                        if (refPath == targetDepPath)
                        {
                            string namePath = GetObjectHierarchyPath(obj);
                            results.Add($"Object: [{obj.GetType().Name}] {namePath} -> 属性: {sp.propertyPath}");
                        }
                    }
                }
            }
            return string.Join("\n", results);
        }

        /// <summary>
        /// 获取 Object 在场景/Prefab 中的层级路径
        /// </summary>
        private static string GetObjectHierarchyPath(Object obj)
        {
            if (obj is GameObject go)
            {
                return GetGameObjectPath(go.transform);
            }
            else if (obj is Component comp)
            {
                return GetGameObjectPath(comp.transform);
            }
            return obj.name;
        }

        private static string GetGameObjectPath(Transform t)
        {
            string path = t.name;
            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }
            return path;
        }

        
        /// <summary>
        /// 检查白名单
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsAllowedExternalDependency(string path)
        {
            // Unity Package
            if (path.StartsWith("Packages/"))
                return true;

            // Unity 内置资源
            if (path.StartsWith("Resources/unity_builtin_extra"))
                return true;

            // Shader
            if (path.EndsWith(".shader"))
                return true;

            // Lighting / GI / 场景自动生成
            if (path.Contains("LightingData"))
                return true;

            return false;
        }
        

        /// <summary>
        /// 搜集打包 bundle 的信息
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, List<string>> Collect()
        {
            // 获取所以在打包设置的文件列表
            ms_collectBuildSettingFile.Start();
            HashSet<string> files =  BuildSetting.Collect();
            ms_collectBuildSettingFile.Stop();
            
            // 收集所有文件的依赖关系
            ms_collectDependencyProfiler.Start();
            Dictionary<string,List<string>> dependencyDic = CollectDependency(files);
            ms_collectDependencyProfiler.Stop();
            
            CheckExternalDependencies(BuildSetting, files);

            // 标记所有资源的信息
            Dictionary<string, EResourceType> assetDic = new Dictionary<string, EResourceType>();
            
            // 被打包配置分析到的直接设置为 Oirect
            foreach (string url in files)
            {
                assetDic.Add(url, EResourceType.Direct);
            }
            

            // 依赖的资源标记为 Dependency, 已经存在的说明就是 Direct 的资源
            foreach (string url in dependencyDic.Keys)
            {
                if (!assetDic.ContainsKey(url))
                {
                    assetDic.Add(url, EResourceType.Dependency);
                }
            }
            
            // 该字段保存Bundle对应的资源集合
            ms_collectBundleProfiler.Start();
            Dictionary<string, List<string>> bundleDic = CollectBundle(BuildSetting, assetDic, dependencyDic);
            ms_collectBundleProfiler.Stop();
            
            // 生成 manifest 文件
            ms_GenerateManifestPofiler.Start();
            GenerateManifest(assetDic, bundleDic, dependencyDic);
            ms_GenerateManifestPofiler.Stop();
            
            return bundleDic;
        }

        /// <summary>
        /// 收集指定文件集合的所有依赖信息
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private static Dictionary<string, List<string>> CollectDependency(ICollection<string> files)
        {
            float min = ms_GetDependencyProgress.x;
            float max = ms_GetDependencyProgress.y;

            Dictionary<string, List<string>> dependencyDic = new Dictionary<string, List<string>>();
            
            // 声明 filelist 后，不需要的拒了
            List<string> fileList = new List<string>(files);

            for (int i = 0; i < fileList.Count; i++)
            {
                string assetUrl = fileList[i];
                if (dependencyDic.ContainsKey(assetUrl)) continue;

                if (i % 10 == 0)
                {
                    // 模拟大概进度
                    float progress = min + (max - min) * ((float)i / files.Count * 3);
                    EditorUtility.DisplayProgressBar($"{(nameof(CollectDependency))}", "搜集依赖信息", progress);
                }

                string[] dependencies = AssetDatabase.GetDependencies(assetUrl, false);
                List<string> dependencyList = new List<string>(dependencies.Length);

                // 过滤掉不符合要求的 asset
                for (int ii = 0; ii < dependencies.Length; ii++)
                {
                    string tempAssetUrl = dependencies[ii];
                    string extension = Path.GetExtension(tempAssetUrl).ToLower();
                    if (string.IsNullOrEmpty(extension) || extension == ".cs" || extension == ".dll") continue;
                    dependencyList.Add(tempAssetUrl);
                    if(!fileList.Contains(tempAssetUrl)) fileList.Add(tempAssetUrl);
                }
                
                dependencyDic.Add(assetUrl,dependencyList);
            }

            return dependencyDic;
        }

        private static Dictionary<string, List<string>> CollectBundle(BuildSetting buildSetting,
            Dictionary<string, EResourceType> assetDic, Dictionary<string, List<string>> dependencyDic)
        {
            float min = ms_CollectBundleInfoProgress.x;
            float max = ms_CollectBundleInfoProgress.y;

            EditorUtility.DisplayProgressBar($"{nameof(CollectBundle)}", "搜集 bundle 信息", min);
            
            Dictionary<string, List<string>> bundleDic = new Dictionary<string, List<string>>();
            
            // 外部资源
            List<string> notInRuleList = new List<string>();

            int index = 0;
            foreach (KeyValuePair<string,EResourceType> pair in assetDic)
            {
                index++;
                string assetUrl = pair.Key;
                string bundleName = buildSetting.GetBundleName(assetUrl, pair.Value);

                /*
                if (assetUrl.StartsWith("Packages/"))
                {
                    Debug.LogWarning($"{assetUrl}为Package资源");
                    continue;
                }
                */

                // 没有 bundleName 的资源为外部资源
                if (bundleName == null)
                {
                    notInRuleList.Add(assetUrl);
                    continue;
                }

                List<string> list;
                if (!bundleDic.TryGetValue(bundleName, out list))
                {
                    list = new List<string>();
                    bundleDic.Add(bundleName, list);
                }
                
                list.Add(assetUrl);

                EditorUtility.DisplayProgressBar($"{nameof(CollectBundle)}", "搜集 bundle 信息",
                    min + (max - min) * ((float)index / assetDic.Count));
            }

            if (notInRuleList.Count > 0)
            {
                string message = string.Empty;
                for (int i = 0; i < notInRuleList.Count; i++)
                {
                    message += "\n" + notInRuleList[i];
                }
                EditorUtility.ClearProgressBar();
                throw new Exception($"资源不在打包规则，或者后缀不匹配{message}");
            }

            foreach (List<string> list in bundleDic.Values)
            {
                list.Sort();
            }

            return bundleDic;
        }

        /// <summary>
        /// 生成资源描述文件
        /// </summary>
        /// <param name="assetDic">资源类型字典</param>
        /// <param name="bundleDic">bundle 数据字典</param>
        /// <param name="dependencyDic">依赖信息字典</param>
        private static void GenerateManifest(Dictionary<string, EResourceType> assetDic,
            Dictionary<string, List<string>> bundleDic, Dictionary<string,List<string>> dependencyDic)
        {
            float min = ms_GetDependencyProgress.x;
            float max = ms_GetDependencyProgress.y;
            
            EditorUtility.DisplayProgressBar($"{nameof(GetMainBranchMessage)}", "生成打包信息", min);
            
            // 生成临时存放文件的目录
            if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            
            // 资源映射 id
            Dictionary<string, ushort> assetIdDic = new Dictionary<string, ushort>();

            #region 生成资源描述信息
            {
                // 删除资源描述文本文件
                if(File.Exists(resourcePath_Text)) File.Delete(resourcePath_Text);
                
                // 删除资源描述
                if(File.Exists(resourcePath_Binary)) File.Delete(resourcePath_Binary);
                
                // 写入资源列表
                StringBuilder resourceSb = new StringBuilder(); // 给人看的（用于排查问题用）
                MemoryStream resourceMs = new MemoryStream();
                BinaryWriter resourceBw = new BinaryWriter(resourceMs); // 机器保存的
                if (assetDic.Count > ushort.MaxValue) // 控制最大资源数不超过 65535
                {
                    EditorUtility.ClearProgressBar();
                    throw new Exception($"资源个数超出{ushort.MaxValue}");
                }
                
                // 写入个数
                resourceBw.Write((ushort)assetDic.Count);
                Debug.Log($"写入资源个数{assetDic.Count}");
                List<string> keys = new List<string>(assetDic.Keys);
                keys.Sort();

                for (ushort i = 0; i < keys.Count; i++)
                {
                    string assetUrl = keys[i];
                    assetIdDic.Add(assetUrl, i); // 资源映射 id
                    resourceSb.AppendLine($"{i}\t{assetUrl}");
                    resourceBw.Write(assetUrl);
                }
                resourceMs.Flush();
                byte[] buffer = resourceMs.GetBuffer();
                resourceBw.Close();
                
                // 写入资源描述文本文件
                File.WriteAllText(resourcePath_Text, resourceSb.ToString(), Encoding.UTF8); // 资源 id 与路径的映射
                File.WriteAllBytes(resourcePath_Binary, buffer);
                /* 二进制文件保存形式
                 * [资源数量] assetDic.Count
                 * [资源路径 0] assetUrl
                 * [资源路径 1] assetUrl
                 * ...
                 * [资源路径 count - 1] assetUrl
                 *
                 * 其中这个 id 用户可以通过 resourcePath_Text 中阅读
                 */
            }
            #endregion

            EditorUtility.DisplayProgressBar($"{nameof(GetMainBranchMessage)}", "生成打包信息", min + (max - min) * 0.3f);

            #region 生成 Bundle 描述信息
            {
                // 删除 Bundle 描述文本文件
                if(File.Exists(bundlePath_Text)) File.Delete(bundlePath_Text);
                
                // 删除 Bundle 描述
                if(File.Exists(bundlePath_Binary)) File.Delete(bundlePath_Binary);
                
                // 写入 Bundle 列表
                StringBuilder bundleSb = new StringBuilder();
                MemoryStream bundleMs = new MemoryStream();
                BinaryWriter bundleBw = new BinaryWriter(bundleMs);

                
                // 写入 bundle 个数
                bundleBw.Write((ushort)bundleDic.Count);

                foreach (var kv in bundleDic)
                {
                    string bundleName = kv.Key;
                    List<string> assets = kv.Value;
                    
                    // 写入 bundle
                    bundleSb.AppendLine(bundleName);
                    bundleBw.Write(bundleName);
                    
                    // 写入资源个数
                    bundleBw.Write((ushort)assets.Count);

                    for (int i = 0; i < assets.Count; i++)
                    {
                        string assetUrl = assets[i];
                        ushort assetId = assetIdDic[assetUrl];
                        bundleSb.AppendLine($"\t{assetUrl}");
                        bundleBw.Write(assetId);
                    }
                    
                    
                }
                bundleMs.Flush();
                byte[] buffer = bundleMs.GetBuffer();
                bundleBw.Close();
                
                // 写入资源描述文本文件
                File.WriteAllText(bundlePath_Text, bundleSb.ToString(), Encoding.UTF8);
                File.WriteAllBytes(bundlePath_Binary, buffer);
                
                /* 二进制文件保存形式
                 * [bundleDic.count]        bundle 的总数
                 *      [bundleName]        bundle 名称
                 *          [assetCount]    bundle 需要的资源数量
                 *              [assetId]   资源对应的 id
                 * ...
                 */
            }
            #endregion
            
            EditorUtility.DisplayProgressBar($"{nameof(GetMainBranchMessage)}", "生成打包信息", min + (max - min) * 0.8f);

            #region 生成资源依赖描述信息
            {
                // 删除资源依赖描述文本文件
                if(File.Exists(dependencyPath_Text)) File.Delete(dependencyPath_Text);
                
                // 删除资源依赖描述
                if(File.Exists(dependencyPath_Binary)) File.Delete(dependencyPath_Binary);
                
                // 写入资源依赖
                StringBuilder dependencySb = new StringBuilder();
                MemoryStream dependencyMs = new MemoryStream();
                BinaryWriter dependencyBw = new BinaryWriter(dependencyMs);

                // 用于保存资源依赖链
                List<List<ushort>> dependencyList = new List<List<ushort>>();
                foreach (var kv in dependencyDic)
                {
                    // 获取当前资源的依赖资源地址
                    List<string> dependencyAssets = kv.Value;

                    // 如果该资源无依赖资源
                    if (dependencyAssets.Count == 0) continue;

                    string assetUrl = kv.Key;

                    // 记录依赖资源路径映射的 id
                    List<ushort> ids = new List<ushort>();
                    // 将自己 id 先放入依赖数组 0 下标中，后面是依赖的 id
                    // 如数组元素 [A,B,C,D]. 解析为 A 依赖 B,C,D
                    ids.Add(assetIdDic[assetUrl]);

                    string content = assetUrl;
                    for (int i = 0; i < dependencyAssets.Count; i++)
                    {
                        string dependencyAssetUrl = dependencyAssets[i];
                        content += $"\t{dependencyAssetUrl}";
                        ids.Add(assetIdDic[dependencyAssetUrl]);
                    }

                    dependencySb.AppendLine(content);

                    // 如果资源依赖超出 255个资源
                    if (ids.Count > byte.MaxValue)
                    {
                        EditorUtility.ClearProgressBar();
                        throw new Exception($"资源{assetUrl}的依赖超出一个字节的上限:{byte.MaxValue}");
                    }
                    
                    dependencyList.Add(ids);
                }
                
                // 写入依赖链个数
                dependencyBw.Write((ushort)dependencyList.Count);
                for (int i = 0; i < dependencyList.Count; i++)
                {
                    // 写入资源数
                    List<ushort> ids = dependencyList[i];
                    dependencyBw.Write((ushort)ids.Count);
                    
                    for(int ii = 0; ii < ids.Count; ii++) dependencyBw.Write(ids[ii]);
                }
                
                dependencyMs.Flush();
                byte[] buffer = dependencyMs.GetBuffer();
                dependencyBw.Close();
                
                // 写入资源描述文本文件
                File.WriteAllText(dependencyPath_Text, dependencySb.ToString(), Encoding.UTF8);
                File.WriteAllBytes(dependencyPath_Binary, buffer);
                
                /* 二进制文件保存形式
                 * [dependencyList.count]   依赖链数量
                 *      [ids.count]         当前依赖链依赖资源数量
                 *          [id index = 0]  第一个下标为当前资源路径映射的 id
                 *          [id index > 0]  后面都是依赖的资源路径映射 id
                 * ...
                 */
            }
            #endregion
            
            AssetDatabase.Refresh(); // 刚刚在磁盘上新建 / 修改 / 删除的文件同步到 Unity 编辑器里。
            
            EditorUtility.DisplayProgressBar($"{nameof(GenerateManifest)}", "生成打包信息", max);
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 打包 AssetBundle
        /// </summary>
        /// <param name="bundleDic"></param>
        /// <returns></returns>
        public static AssetBundleManifest BuildBundle(Dictionary<string,List<string>> bundleDic)
        {
            float min = ms_BuildBundleProgress.x;
            float max = ms_BuildBundleProgress.y;
            
            EditorUtility.DisplayProgressBar($"{nameof(BuildBundle)}", "打包 AssetBundle", min);

            if (!Directory.Exists(buildPath)) Directory.CreateDirectory(buildPath);

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(buildPath, GetBuilds(bundleDic),
                BuildAssetBundleOptions, EditorUserBuildSettings.activeBuildTarget);
            
            EditorUtility.DisplayProgressBar($"{nameof(BuildBundle)}", "打包 AssetBundle", max);

            return manifest;
        }

        /// <summary>
        /// 清空多余的 AssetBundle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bundleDic"></param>
        public static void ClearAssetBundle(string path, Dictionary<string, List<string>> bundleDic)
        {
            float min = ms_ClearBundleProgress.x;
            float max = ms_ClearBundleProgress.y;
            
            EditorUtility.DisplayProgressBar($"{nameof(ClearAssetBundle)}", "清除多余的 AssetBundle 文件", min);

            List<string> fileList = GetFiles(path, null, null);
            HashSet<string> fileSet = new HashSet<string>(fileList);
            foreach (string bundle in bundleDic.Keys)
            {
                fileSet.Remove($"{path}{bundle}");
                fileSet.Remove($"{path}{bundle}{BUNDLE_MANIFEST_SUFFIX}");
            }
            
            fileSet.Remove($"{path}{PLATFORM}");
            fileSet.Remove($"{path}{PLATFORM}{BUNDLE_MANIFEST_SUFFIX}");

            Parallel.ForEach(fileSet, ParallelOptions, File.Delete);
            
            EditorUtility.DisplayProgressBar($"{nameof(ClearAssetBundle)}", "清除多余的 AssetBundle 文件", max);
        }

        private static void BuildManifest()
        {
            float min = ms_BuildManifestProgress.x;
            float max = ms_BuildManifestProgress.y;

            EditorUtility.DisplayProgressBar($"{nameof(BuildManifest)}", "将 Manifest 打包为 AssetBundle", min);

            if (!Directory.Exists(TempBuildPath)) Directory.CreateDirectory(TempBuildPath);
            
            string prefix = Application.dataPath.Replace("/Assets", "/").Replace("\\", "/");

            AssetBundleBuild manifest = new AssetBundleBuild();
            manifest.assetBundleName = $"{MANIFEST}{BUNDLE_SUFFIX}";
            manifest.assetNames = new string[3]
            {
                resourcePath_Binary.Replace(prefix, ""),
                bundlePath_Binary.Replace(prefix, ""),
                dependencyPath_Binary.Replace(prefix, ""),
            };

            EditorUtility.DisplayProgressBar($"{nameof(BuildManifest)}", "将 Manifest 打包为 AssetBundle",
                min + (max - min) * 0.5f);

            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(TempBuildPath,
                new AssetBundleBuild[] { manifest }, BuildAssetBundleOptions,
                EditorUserBuildSettings.activeBuildTarget);
            
            // 把文件拷贝到 Build 目录
            if (assetBundleManifest)
            {
                string manifestFile = $"{TempBuildPath}/{MANIFEST}{BUNDLE_SUFFIX}";
                string target = $"{buildPath}/{MANIFEST}{BUNDLE_SUFFIX}";
                if (File.Exists(manifestFile))
                {
                    File.Copy(manifestFile, target);
                }
            }
            
            // 删除临时目录
            if (!Directory.Exists(TempBuildPath)) Directory.Delete(TempBuildPath, true);
            EditorUtility.DisplayProgressBar($"{nameof(BuildManifest)}", "将 Manifest 打包为 AssetBundle", max);
        }

        /// <summary>
        /// 获取所有需要打包的 AssetBundleBuild
        /// </summary>
        /// <param name="bundleTable"></param>
        /// <returns></returns>
        private static AssetBundleBuild[] GetBuilds(Dictionary<string, List<string>> bundleTable)
        {
            int index = 0;
            AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[bundleTable.Count];
            
            foreach (KeyValuePair<string,List<string>> pair in bundleTable)
            {
                assetBundleBuilds[index++] = new AssetBundleBuild()
                {
                    assetBundleName = pair.Key,
                    assetNames = pair.Value.ToArray()
                };
            }

            return assetBundleBuilds;
        }
        
        /// <summary>
        /// 获取指定路径的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="prefix">后缀</param>
        /// <param name="suffixes"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string path, string prefix, string[] suffixes)
        {
            string[] files = Directory.GetFiles(path, $"*.*", SearchOption.AllDirectories);
            List<string> result = new List<string>(files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i].Replace("\\", "/");

                if (prefix != null && file.StartsWith(prefix, StringComparison.InvariantCulture)) continue;

                if (suffixes != null && suffixes.Length > 0)
                {
                    bool exist = false;
                    for (int ii = 0; ii < suffixes.Length; ii++)
                    {
                        string suffix = suffixes[ii];
                        if (file.EndsWith(suffix, StringComparison.InvariantCulture))
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist) continue;
                }
                result.Add(file);
            }
            
            return result;
        }

        #endregion
    }
}
