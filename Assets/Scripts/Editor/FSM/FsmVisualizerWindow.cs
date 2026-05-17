using System.Linq;
using GameFSM.FSM;
using UnityEditor;
using UnityEngine;

namespace Editor.FSM
{
    public class FsmVisualizerWindow : EditorWindow
    {
        [MenuItem("Tools/FSM 可视化")]
        public static void ShowWindow() => GetWindow<FsmVisualizerWindow>("Global FSM");

        private Vector2 _scrollPos;
        private string _selectedFsmId; // 当前窗口选中的 FSM ID

        void OnGUI()
        {
            var allFsms = FsmDebuggerRegistry.GetAll();

            if (allFsms.Count == 0)
            {
                EditorGUILayout.HelpBox("当前没有活跃的状态机 (请在运行模式下查看)", MessageType.Info);
                return;
            }

            EditorGUILayout.BeginHorizontal();

            // 左侧栏：活跃状态机列表
            DrawLeftPanel(allFsms);

            // 右侧栏：详细状态信息
            DrawRightPanel(allFsms);

            EditorGUILayout.EndHorizontal();

            if (Application.isPlaying) Repaint();
        }

        private void DrawLeftPanel(System.Collections.Generic.Dictionary<string, IFsmDebugger> allFsms)
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(200));
            GUILayout.Label("活跃武器列表", EditorStyles.boldLabel);

            foreach (var id in allFsms.Keys.ToList())
            {
                if (GUILayout.Toggle(_selectedFsmId == id, id, "Button"))
                {
                    _selectedFsmId = id;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawRightPanel(System.Collections.Generic.Dictionary<string, IFsmDebugger> allFsms)
        {
            EditorGUILayout.BeginVertical("box");
        
            if (!string.IsNullOrEmpty(_selectedFsmId) && allFsms.TryGetValue(_selectedFsmId, out var fsm))
            {
                GUILayout.Label($"详情: {_selectedFsmId}", EditorStyles.boldLabel);
                EditorGUILayout.Space(5);
            
                string current = fsm.GetCurrentStateName();
                EditorGUILayout.LabelField("当前状态:", current, EditorStyles.whiteLargeLabel);
            
                EditorGUILayout.Space(10);
                foreach (var stateName in fsm.GetAllStateNames())
                {
                    bool isActive = (stateName == current);
                    GUI.color = isActive ? Color.green : Color.white;
                    EditorGUILayout.LabelField(isActive ? "▶ " + stateName : "  " + stateName, EditorStyles.toolbarButton);
                    GUI.color = Color.white;
                
                    foreach (var cond in fsm.GetConditions(stateName))
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField(cond, EditorStyles.miniLabel);
                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("在左侧选择一个武器查看详情", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }
    }
}