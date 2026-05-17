using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GameFSM.LevelFSM;
using UnityEditor;
using UnityEngine;

// 引用你的状态机命名空间

namespace Editor.FSM
{
    public class FSMHierarchyViewer : EditorWindow
    {
        private GameObject _targetObject;
        private Vector2 _scrollPos;
        private GUIStyle _stateStyle;
        private GUIStyle _activeStateStyle;
        private GUIStyle _conditionStyle;
        private HashSet<object> _visited = new HashSet<object>();

        [MenuItem("Tools/FSMLevel 可视化")]
        public static void ShowWindow() => GetWindow<FSMHierarchyViewer>("FSM Debugger");

        private void OnEnable()
        {
            // 初始化样式
            _stateStyle = new GUIStyle();
            _stateStyle.normal.textColor = Color.white;

            _activeStateStyle = new GUIStyle();
            _activeStateStyle.normal.textColor = Color.green;
            _activeStateStyle.fontStyle = FontStyle.Bold;

            _conditionStyle = new GUIStyle();
            _conditionStyle.normal.textColor = new Color(0.5f, 0.8f, 1f);
            _conditionStyle.fontSize = 10;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            _targetObject = (GameObject)EditorGUILayout.ObjectField("目标角色:", _targetObject, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            if (_targetObject == null)
            {
                EditorGUILayout.HelpBox("请选择挂载了 PlayerController 的物体", MessageType.Info);
                return;
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            _visited.Clear();

            // 1. 获取物体上所有的脚本组件
            var components = _targetObject.GetComponents<MonoBehaviour>();
            foreach (var comp in components)
            {
                if (comp == null) continue;
                // 2. 从组件开始向下深度搜索字段
                InspectObjectRecursive(comp, comp.GetType().Name);
            }

            EditorGUILayout.EndScrollView();

            // 播放模式下强制刷新 UI
            if (Application.isPlaying) Repaint();
        }

        /// <summary>
        /// 深度搜索对象内部的 FSM 实例
        /// </summary>
        private void InspectObjectRecursive(object obj, string path)
        {
            if (obj == null || _visited.Contains(obj)) return;
            
            Type type = obj.GetType();
            // 过滤系统库和Unity内置库，优化性能
            if (type.IsPrimitive || type == typeof(string) || (type.Namespace != null && type.Namespace.StartsWith("UnityEngine."))) return;
            
            _visited.Add(obj);

            // 检查这个对象本身是否就是状态机节点
            if (IsSubclassOfRawGeneric(typeof(AbstractLevelState<>), type))
            {
                DrawFSMHeader(path, obj);
                return;
            }

            // 如果不是状态机，则反射其内部所有字段继续寻找
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                object val = field.GetValue(obj);
                if (val == null) continue;

                // 递归探测
                InspectObjectRecursive(val, $"{path} -> {field.Name}");
            }
        }

        private void DrawFSMHeader(string path, object fsmInstance)
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Found: {path}", EditorStyles.miniBoldLabel);
            GUILayout.Space(2);
            DrawStateNodeRecursive(fsmInstance, 0, true);
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        /// <summary>
        /// 递归绘制状态机树状 UI
        /// </summary>
        private void DrawStateNodeRecursive(object state, int indent, bool isActivePath)
        {
            if (state == null) return;

            // 获取数据 (反射属性和私有变量)
            object currentState = GetReflectedProperty(state, "CurrentState");
            IDictionary statesDict = GetReflectedProperty(state, "States") as IDictionary;
            IEnumerable conditions = GetReflectedFieldInBase(state, "Conditions") as IEnumerable;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indent * 20);
            
            // 绘制当前状态名
            string stateName = state.GetType().Name;
            string prefix = (statesDict != null && statesDict.Count > 0) ? "▼ " : "● ";
            EditorGUILayout.LabelField(prefix + stateName, isActivePath ? _activeStateStyle : _stateStyle);
            
            EditorGUILayout.EndHorizontal();

            // 绘制该层级的转换条件
            if (conditions != null && isActivePath) // 只显示当前激活状态的转换条件，减少视觉干扰
            {
                foreach (var cond in conditions)
                {
                    string nextName = GetReflectedProperty(cond, "NextStateName")?.ToString();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(indent * 20 + 25);
                    EditorGUILayout.LabelField($"➔ [To: {nextName}]", _conditionStyle);
                    EditorGUILayout.EndHorizontal();
                }
            }

            // 如果是复合状态，递归子状态
            if (statesDict != null)
            {
                foreach (DictionaryEntry entry in statesDict)
                {
                    object subState = entry.Value;
                    bool subStateActive = isActivePath && (subState == currentState);
                    DrawStateNodeRecursive(subState, indent + 1, subStateActive);
                }
            }
        }

        #region 反射辅助工具

        private bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur) return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        private object GetReflectedProperty(object obj, string name)
        {
            return obj?.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(obj);
        }

        private object GetReflectedFieldInBase(object obj, string name)
        {
            if (obj == null) return null;
            Type t = obj.GetType();
            FieldInfo field = null;
            // 向上查找基类字段 (如 Conditions)
            while (t != null && field == null)
            {
                field = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }
            return field?.GetValue(obj);
        }

        #endregion
    }
}