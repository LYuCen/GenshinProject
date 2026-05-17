using mSystem.BuffSystem.ElementBuff;
using UnityEditor;
using UnityEngine;

namespace Editor.ElementEditor
{
    [CustomPropertyDrawer(typeof(ElementData))]
    public class ElementDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 计算布局
            float width = position.width / 4;
            Rect enableRect = new Rect(position.x, position.y, 20, position.height);
            Rect valRect = new Rect(position.x + 25, position.y, width * 1.5f, position.height);
            Rect levelRect = new Rect(position.x + 30 + width * 1.5f, position.y, width, position.height);

            // 绘制字段
            SerializedProperty enable = property.FindPropertyRelative("Enable");
            SerializedProperty val = property.FindPropertyRelative("Val");
            SerializedProperty level = property.FindPropertyRelative("Level");

            EditorGUI.PropertyField(enableRect, enable, GUIContent.none);
        
            // 只有开启时才显示数值
            if (enable.boolValue)
            {
                EditorGUI.LabelField(valRect, "Val: " + val.intValue);
                EditorGUI.LabelField(levelRect, "Lv: " + level.intValue);
            }
            else
            {
                EditorGUI.LabelField(valRect, "(Empty)", EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUI.EndProperty();
        }
    }
}