using Player.AnimPro;
using UnityEditor;
using UnityEditor.Callbacks; // 必须引用
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class AnimGraphWindow : EditorWindow
    {
        private AnimGraphView _graphView;
        private AnimGraphData _currentAsset; // 记录当前正在编辑的资源

        [MenuItem("Window/AnimTree Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<AnimGraphWindow>();
            window.titleContent = new GUIContent("AnimTree Editor");
        }

        // --- 核心功能：双击资源文件回调 ---
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            // 1. 将实例 ID 转换为实际的对象
            Object asset = EditorUtility.InstanceIDToObject(instanceID);

            // 2. 判断该对象是否为我们的动画图数据
            if (asset is AnimGraphData data)
            {
                // 3. 打开窗口
                var window = GetWindow<AnimGraphWindow>();
                window.titleContent = new GUIContent("AnimTree Editor - " + data.name);
                
                // 4. 将资源传递给窗口实例并加载
                window.LoadAsset(data);
                return true; // 返回 true 表示我们处理了这个打开动作
            }

            return false; // 返回 false 表示让 Unity 用默认方式处理（或不处理）
        }

        private void CreateGUI()
        {
            _graphView = new AnimGraphView(this);
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);

            GenerateToolbar(_graphView);

            // 如果已经有资源被选中了（比如通过双击打开的），立即加载
            if (_currentAsset != null)
            {
                _graphView.LoadGraph(_currentAsset);
            }
        }

        // 外部调用的加载方法
        public void LoadAsset(AnimGraphData data)
        {
            _currentAsset = data;

            // 如果画布已经初始化好了，直接加载
            if (_graphView != null)
            {
                _graphView.LoadGraph(data);
            }
        }

        private void GenerateToolbar(AnimGraphView graphView)
        {
            var toolbar = new UnityEditor.UIElements.Toolbar();

            // 保存按钮逻辑
            var saveBtn = new Button(() => {
                if (_currentAsset != null)
                {
                    graphView.SaveGraph(_currentAsset);
                }
                else
                {
                    // 如果是空窗口，提示先 Save As
                    SaveAs(graphView);
                }
            }) { text = "Save" };

            var saveAsBtn = new Button(() => SaveAs(graphView)) { text = "Save As" };

            toolbar.Add(saveBtn);
            toolbar.Add(saveAsBtn);
            rootVisualElement.Add(toolbar);
        }

        private void SaveAs(AnimGraphView graphView)
        {
            string path = EditorUtility.SaveFilePanelInProject("Save AnimGraph", "NewAnimGraph", "asset", "Save Data");
            if (!string.IsNullOrEmpty(path))
            {
                var newData = ScriptableObject.CreateInstance<AnimGraphData>();
                graphView.SaveGraph(newData);
                AssetDatabase.CreateAsset(newData, path);
                _currentAsset = newData;
            }
        }
        
        private void OnEnable()
        {
            // 只有在这里注册，按下 Ctrl+Z 时 UI 才会刷新
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            if (_currentAsset != null && _graphView != null)
            {
                // 撤销后，数据变了，UI 方块没变，所以必须重新加载一次
                _graphView.LoadGraph(_currentAsset);
            }
        }
    }
}