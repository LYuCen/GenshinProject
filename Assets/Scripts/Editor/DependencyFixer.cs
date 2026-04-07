using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class DependencyFixer : EditorWindow
    {
        private string targetPath = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Lit.mat";
        private Material replacementMaterial;

        [MenuItem("Tools/ResBuild/Dependency Fixer")]
        public static void ShowWindow()
        {
            GetWindow<DependencyFixer>("资源引用修复器");
        }

        private void OnGUI()
        {
            GUILayout.Label("目标资源路径 (查找这个):", EditorStyles.boldLabel);
            targetPath = EditorGUILayout.TextField(targetPath);

            GUILayout.Space(10);
            GUILayout.Label("替换为 (你的本地材质):", EditorStyles.boldLabel);
            replacementMaterial = (Material)EditorGUILayout.ObjectField(replacementMaterial, typeof(Material), false);

            if (GUILayout.Button("第一步：查找引用了该路径的资源"))
            {
                FindAllReferences();
            }

            if (GUILayout.Button("第二步：一键替换引用 (慎用！建议先备份)"))
            {
                ReplaceAllReferences();
            }
        }

        private List<string> FindAllReferences()
        {
            List<string> references = new List<string>();
            // 只搜索你的 AssetBundle 目录
            string[] allGuids = AssetDatabase.FindAssets("t:Prefab t:Material", new[] { "Assets/AssetBundle" });
        
            Debug.Log($"正在扫描 Assets/AssetBundle 下的 {allGuids.Length} 个资源...");

            foreach (string guid in allGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string[] dependencies = AssetDatabase.GetDependencies(path);

                foreach (string dep in dependencies)
                {
                    if (dep == targetPath)
                    {
                        references.Add(path);
                        Debug.Log($"<color=yellow>发现引用:</color> {path}", AssetDatabase.LoadAssetAtPath<Object>(path));
                        break;
                    }
                }
            }

            Debug.Log($"扫描完成，共发现 {references.Count} 个资源引用了目标。");
            return references;
        }

        private void ReplaceAllReferences()
        {
            if (replacementMaterial == null)
            {
                EditorUtility.DisplayDialog("错误", "请先指定一个本地材质作为替换资源！", "确定");
                return;
            }

            List<string> refs = FindAllReferences();
            int count = 0;

            foreach (string path in refs)
            {
                // 加载预制体资源
                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefabAsset == null) continue;

                // 在内存中实例化，用于修改
                GameObject instance = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;
                bool changed = false;

                // 获取所有 Renderer
                Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers)
                {
                    // 使用序列化对象直接操作属性，防止 Unity 丢弃修改
                    SerializedObject so = new SerializedObject(renderer);
                    SerializedProperty materialsProp = so.FindProperty("m_Materials");

                    for (int i = 0; i < materialsProp.arraySize; i++)
                    {
                        SerializedProperty matElem = materialsProp.GetArrayElementAtIndex(i);
                        Object currentMat = matElem.objectReferenceValue;

                        // 检查路径是否匹配
                        if (currentMat != null && AssetDatabase.GetAssetPath(currentMat) == targetPath)
                        {
                            matElem.objectReferenceValue = replacementMaterial;
                            changed = true;
                            Debug.Log($"[已替换] 预制体: {path} -> 节点: {renderer.name}");
                        }
                    }
                    if (changed) so.ApplyModifiedProperties(); // 强制应用修改
                }

                if (changed)
                {
                    // 关键：将修改后的实例写回预制体资源
                    PrefabUtility.SaveAsPrefabAsset(instance, path);
                    count++;
                }
                DestroyImmediate(instance);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("完成", $"成功修复了 {count} 个预制体！", "确定");
        }
    }
}