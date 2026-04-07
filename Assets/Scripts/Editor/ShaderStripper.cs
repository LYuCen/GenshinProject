using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;

namespace Editor
{
    public class ShaderStripper
    {
        public int callbackOrder => 0;

        private static HashSet<string> _whiteListVariants = new HashSet<string>();
        private static bool _isInitialized = false;

        // 确保这里的路径和你保存的 SVC 路径一致
        private const string SVC_PATH = "Assets/AssetBundle/Shaders/NewShaderVariants.shadervariants";

        private void Initialize()
        {
            if (_isInitialized) return;

            ShaderVariantCollection svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(SVC_PATH);
            if (svc == null)
            {
                Debug.LogError($"[ShaderStripper] 找不到 SVC 文件: {SVC_PATH}。裁剪脚本可能导致全屏粉红！");
                return;
            }

            _whiteListVariants.Clear();
            SerializedObject so = new SerializedObject(svc);
            SerializedProperty shadersProp = so.FindProperty("m_Shaders");

            for (int i = 0; i < shadersProp.arraySize; i++)
            {
                SerializedProperty shaderEntry = shadersProp.GetArrayElementAtIndex(i);
                Shader shader = shaderEntry.FindPropertyRelative("first").objectReferenceValue as Shader;
                SerializedProperty variantsProp = shaderEntry.FindPropertyRelative("second.variants");

                for (int j = 0; j < variantsProp.arraySize; j++)
                {
                    SerializedProperty variantProp = variantsProp.GetArrayElementAtIndex(j);
                    string keywords = variantProp.FindPropertyRelative("keywords").stringValue;
                    int passType = variantProp.FindPropertyRelative("passType").intValue;

                    // 【关键修改】：对白名单里的关键字进行排序
                    string sortedKeywords = SortKeywords(keywords);
                    string key = $"{shader.name}_{passType}_{sortedKeywords}";
                    _whiteListVariants.Add(key);
                }
            }

            _isInitialized = true;
            Debug.Log($"[ShaderStripper] 白名单初始化成功，共加载 {_whiteListVariants.Count} 个合法变体。");
        }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            if (shader.name != "Universal Render Pipeline/Lit") return;

            Initialize();

            int initialCount = data.Count;
            for (int i = data.Count - 1; i >= 0; i--)
            {
                // 【关键修改】：对当前打包生成的关键字进行排序
                string currentKeywords = data[i].shaderKeywordSet.ToString();
                string sortedCurrentKeywords = SortKeywords(currentKeywords);
            
                string currentKey = $"{shader.name}_{(int)snippet.passType}_{sortedCurrentKeywords}";

                if (!_whiteListVariants.Contains(currentKey))
                {
                    data.RemoveAt(i);
                }
            }
        }

        // 辅助函数：将关键字字符串按字母排序，统一格式
        private string SortKeywords(string keywords)
        {
            if (string.IsNullOrEmpty(keywords)) return "";
            string[] split = keywords.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(split);
            return string.Join(" ", split);
        }
    }
}