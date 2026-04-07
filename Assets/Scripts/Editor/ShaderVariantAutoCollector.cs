using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Editor
{
    public class ShaderVariantCollectorWindow : EditorWindow
    {
        // 这里可以手动拖拽
        public Shader targetShader;
        public ShaderVariantCollection targetSVC;

        [MenuItem("Tools/Shader/Auto Collect Window")]
        public static void ShowWindow()
        {
            GetWindow<ShaderVariantCollectorWindow>("变体收集工具");
        }

        private void OnGUI()
        {
            GUILayout.Label("自动收集 URP Lit 变体", EditorStyles.boldLabel);

            // 1. 手动指定 Shader (你可以从 Project 面板搜索 "Lit" 然后拖进来)
            targetShader = (Shader)EditorGUILayout.ObjectField("目标 Shader", targetShader, typeof(Shader), false);

            // 2. 手动指定 SVC 文件
            targetSVC = (ShaderVariantCollection)EditorGUILayout.ObjectField("目标 SVC 文件", targetSVC, typeof(ShaderVariantCollection), false);

            if (GUILayout.Button("开始自动收集"))
            {
                if (targetShader == null || targetSVC == null)
                {
                    EditorUtility.DisplayDialog("错误", "请先拖入 Shader 和 SVC 文件！", "确定");
                    return;
                }
                ExecuteCollect();
            }
        }

        private void ExecuteCollect()
        {
            targetSVC.Clear();
        
            // 只保留 URP 兼容的 Pass 类型
            PassType[] types = { PassType.ScriptableRenderPipeline, PassType.ShadowCaster };
        
            // 扫描项目内所有材质球
            string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/AssetBundle/Materials" });
            string[] globalKeywords = { "_MAIN_LIGHT_SHADOWS", "_MAIN_LIGHT_SHADOWS_CASCADE" };

            foreach (var guid in matGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat.shader != targetShader) continue;

                List<string> matKeywords = mat.shaderKeywords.ToList();

                foreach (var passType in types)
                {
                    AddVariantToSVC(targetShader, passType, matKeywords.ToArray(), targetSVC);

                    foreach (var gk in globalKeywords)
                    {
                        var combined = new List<string>(matKeywords);
                        if (!combined.Contains(gk)) combined.Add(gk);
                        AddVariantToSVC(targetShader, passType, combined.ToArray(), targetSVC);
                    }
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green>变体收集成功！</color>");
        }

        private void AddVariantToSVC(Shader shader, PassType passType, string[] keywords, ShaderVariantCollection svc)
        {
            try {
                var variant = new ShaderVariantCollection.ShaderVariant(shader, passType, keywords);
                if (!svc.Contains(variant)) svc.Add(variant);
            } catch { }
        }
    }
}