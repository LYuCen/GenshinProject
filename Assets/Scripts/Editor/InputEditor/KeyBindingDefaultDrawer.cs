using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using mModel.SettingModel;

namespace Editor.InputEditor
{
    [CustomPropertyDrawer(typeof(KeyBindingDefault))]
    public class KeyBindingDefaultDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, bool> Listening = new();

        private const float ButtonWidth = 60;
        private const float Space = 4;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var actionProp = property.FindPropertyRelative("actionName");
            var keyProp = property.FindPropertyRelative("defaultKey");

            string path = property.propertyPath;

            if (!Listening.ContainsKey(path))
                Listening[path] = false;

            float line = EditorGUIUtility.singleLineHeight;

            Rect actionRect = new Rect(
                position.x,
                position.y,
                position.width,
                line);

            Rect keyRect = new Rect(
                position.x,
                position.y + line + Space,
                position.width - ButtonWidth - Space,
                line);

            Rect buttonRect = new Rect(
                keyRect.xMax + Space,
                keyRect.y,
                ButtonWidth,
                line);

            EditorGUI.PropertyField(actionRect, actionProp, new GUIContent("虚拟键"));

            if (Listening[path])
            {
                Listen(keyProp, path);

                EditorGUI.TextField(keyRect, "映射键", "Press Any Key...");

                if (GUI.Button(buttonRect, "取消"))
                {
                    Listening[path] = false;
                }
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);

                EditorGUI.TextField(
                    keyRect,
                    "映射键",
                    ((KeyCode)keyProp.intValue).ToString());

                EditorGUI.EndDisabledGroup();

                if (GUI.Button(buttonRect, "绑定"))
                {
                    foreach (var k in new List<string>(Listening.Keys))
                        Listening[k] = false;

                    Listening[path] = true;

                    GUI.FocusControl(null);
                    EditorGUIUtility.editingTextField = false;
                }
            }

            if (Listening[path])
                RepaintInspector();
        }

        private void Listen(SerializedProperty keyProp, string path)
        {
            Event e = Event.current;

            if (e.type != EventType.KeyDown &&
                e.type != EventType.MouseDown)
                return;

            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Escape)
                {
                    Listening[path] = false;
                }
                else if (e.keyCode != KeyCode.None)
                {
                    keyProp.intValue = (int)e.keyCode;
                    keyProp.serializedObject.ApplyModifiedProperties();

                    Listening[path] = false;
                }

                e.Use();
                return;
            }

            KeyCode mouse = e.button switch
            {
                0 => KeyCode.Mouse0,
                1 => KeyCode.Mouse1,
                2 => KeyCode.Mouse2,
                3 => KeyCode.Mouse3,
                4 => KeyCode.Mouse4,
                _ => KeyCode.None
            };

            if (mouse != KeyCode.None)
            {
                keyProp.intValue = (int)mouse;
                keyProp.serializedObject.ApplyModifiedProperties();

                Listening[path] = false;
            }

            e.Use();
        }

        private static void RepaintInspector()
        {
            if (EditorWindow.focusedWindow != null)
                EditorWindow.focusedWindow.Repaint();
        }

        public override float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + Space + 6;
        }
    }
}