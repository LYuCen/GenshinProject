using System;
using System.Collections.Generic;
using mSystem.BuffSystem.ElementBuff;
using UnityEditor;
using UnityEngine;

namespace Editor.TestEditor
{
    [CustomEditor(typeof(ElementBuffController))]
    public class ElementBuffControllerEditor : UnityEditor.Editor
    {
        // 定义元素名称，方便阅读
        private string[] elementNames = { "None", "Fire(1)", "Water(2)", "Grass(3)", "Electro(4)", "5", "Frost(6)", "7" };

        public override void OnInspectorGUI()
        {
            // 获取目标对象
            ElementBuffController controller = (ElementBuffController)target;

            // --- 1. 绘制默认属性（可选） ---
            DrawDefaultInspector(); 

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("元素量状态 (Elements)", EditorStyles.boldLabel);

            // --- 2. 自定义绘制数组，让它更直观 ---
            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < controller.Elements.Length; i++)
            {
                var data = controller.Elements[i];
                if (data.Enable)
                {
                    // 如果激活，用绿色显示
                    GUI.color = Color.green;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{elementNames[i]}", GUILayout.Width(80));
                EditorGUILayout.LabelField($"Val: {data.Val}", GUILayout.Width(100));
                EditorGUILayout.LabelField($"Lv: {data.Level}", GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();

                GUI.color = Color.white;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // --- 3. 绘制 Dictionary (重点) ---
            EditorGUILayout.LabelField("当前激活的 Buff (Status Effects)", EditorStyles.boldLabel);
        
            // 使用反射或通过控制器公开一个方法来获取私有字典
            // 这里假设我们在运行时查看
            var effects = GetEffectsDict(controller);

            if (effects == null || effects.Count == 0)
            {
                EditorGUILayout.HelpBox("当前无激活中的 Buff", MessageType.Info);
            }
            else
            {
                EditorGUILayout.BeginVertical("helpbox");
                foreach (var kvp in effects)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("• " + kvp.Key.Name, EditorStyles.miniLabel);
                    // 可以在这里根据 IStatusEffect 里的属性显示进度
                    // 例如：EditorGUILayout.LabelField("Finished: " + kvp.Value.IsFinished);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            // --- 4. 添加一些调试按钮 ---
            EditorGUILayout.Space();
            if (Application.isPlaying)
            {
                if (GUILayout.Button("手动清理所有 Buff"))
                {
                    // 你可以在 Controller 里加个这个方法
                    // controller.ClearAllEffects(); 
                }
            }
        
            // 强制重绘，保证运行时数据实时更新
            if (Application.isPlaying)
            {
                Repaint();
            }
        }

        // 利用反射获取私有字段 _statusEffects
        private Dictionary<Type, mSystem.BuffSystem.ElementBuff.StatusEffect.IStatusEffect> GetEffectsDict(ElementBuffController target)
        {
            var field = typeof(ElementBuffController).GetField("_statusEffects", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(target) as Dictionary<Type, mSystem.BuffSystem.ElementBuff.StatusEffect.IStatusEffect>;
        }
    }
}