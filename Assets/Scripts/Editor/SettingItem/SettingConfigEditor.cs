using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using mSystem.mSettingSystem.ItemData;

namespace Editor.SettingItem
{
    [CustomEditor(typeof(SettingConfigSo))]
    public class SettingConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var config = (SettingConfigSo)target;

            if (config.items == null)
                config.items = new List<SettingItemData>();

            EditorGUILayout.Space();

            // =========================
            // ADD
            // =========================
            if (GUILayout.Button("➕ Add Setting Item"))
            {
                config.items.Add(new SettingItemData
                {
                    id = "new_id",
                    name = "New Setting",
                    category = "Default",
                    options = new List<string>(),
                    min = 0,
                    max = 100
                });

                EditorUtility.SetDirty(config);
            }

            EditorGUILayout.Space(10);

            // =========================
            // LIST
            // =========================
            for (int i = 0; i < config.items.Count; i++)
            {
                var item = config.items[i];

                if (item == null)
                {
                    config.items[i] = new SettingItemData();
                    continue;
                }

                EditorGUILayout.BeginVertical("box");

                // =========================
                // HEADER
                // =========================
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"Item {i}", EditorStyles.boldLabel);

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    config.items.RemoveAt(i);
                    EditorUtility.SetDirty(config);
                    GUIUtility.ExitGUI();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                // =========================
                // BASIC
                // =========================
                item.id = EditorGUILayout.TextField("ID", item.id);
                item.name = EditorGUILayout.TextField("Name", item.name);
                item.category = EditorGUILayout.TextField("Category", item.category);

                item.type = (SettingControlType)EditorGUILayout.EnumPopup("Type", item.type);
                item.introduce = EditorGUILayout.TextField("Introduce", item.introduce);

                EditorGUILayout.Space(5);

                // =========================
                // SLIDER
                // =========================
                if (item.type == SettingControlType.Slider)
                {
                    item.min = EditorGUILayout.IntField("Min", item.min);
                    item.max = EditorGUILayout.IntField("Max", item.max);

                    item.value = EditorGUILayout.IntSlider("Value", item.value, item.min, item.max);

                    item.defaultValue = EditorGUILayout.IntField("Default", item.defaultValue);
                }

                // =========================
                // DROPDOWN
                // =========================
                if (item.type == SettingControlType.Dropdown)
                {
                    DrawOptions(item);

                    if (item.options != null && item.options.Count > 0)
                    {
                        item.value = EditorGUILayout.IntSlider(
                            "Selected Index",
                            item.value,
                            0,
                            item.options.Count - 1
                        );

                        EditorGUILayout.LabelField("Current:", item.options[item.value]);
                    }

                    item.defaultValue = EditorGUILayout.IntField("Default Index", item.defaultValue);
                }

                // =========================
                // TOGGLE
                // =========================
                if (item.type == SettingControlType.Toggle)
                {
                    item.value = EditorGUILayout.Toggle("Value", item.value == 1) ? 1 : 0;
                    item.defaultValue = EditorGUILayout.Toggle("Default", item.defaultValue == 1) ? 1 : 0;
                }

                EditorGUILayout.Space(5);

                // =========================
                // DEBUG
                // =========================
                EditorGUILayout.LabelField("Runtime Value:", item.value.ToString());

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(5);
            }

            if (GUI.changed)
                EditorUtility.SetDirty(config);
        }

        // =========================
        // OPTIONS EDITOR
        // =========================
        private void DrawOptions(SettingItemData item)
        {
            if (item.options == null)
                item.options = new List<string>();

            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

            for (int i = 0; i < item.options.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                item.options[i] = EditorGUILayout.TextField(item.options[i]);

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    item.options.RemoveAt(i);
                    GUIUtility.ExitGUI();
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Option"))
            {
                item.options.Add("New Option");
            }
        }
    }
}