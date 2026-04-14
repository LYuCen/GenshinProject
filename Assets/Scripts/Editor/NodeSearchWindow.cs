using System;
using System.Collections.Generic;
using GameFSM.FSM.PlayerFSM;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Player.AnimPro; // 替换成你逻辑节点所在的命名空间

namespace Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private AnimGraphView _graphView;
        private EditorWindow _window;

        public void Init(AnimGraphView graphView, EditorWindow window)
        {
            _graphView = graphView;
            _window = window;
        }

        // 创建菜单列表
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            };

            // 利用 TypeCache (Unity 2019.2+) 自动获取所有节点类
            // 注意：这里要填你的 AnimTreeNode 基类
            var types = TypeCache.GetTypesDerivedFrom(typeof(AnimTreeNode<PlayerManager>));

            foreach (var type in types)
            {
                if (type.IsAbstract) continue;
                tree.Add(new SearchTreeEntry(new GUIContent(type.Name))
                {
                    level = 1,
                    userData = type
                });
            }

            return tree;
        }

        // 当点击菜单中的某一项时
        // NodeSearchWindow.cs 中的 OnSelectEntry 方法

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            var type = (Type)entry.userData;
    
            // 1. 计算鼠标位置
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(
                _window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);

            // 2. 创建一个全新的 NodeData 对象来存储数据
            var nodeData = new NodeData
            {
                GUID = Guid.NewGuid().ToString(),
                NodeType = type.AssemblyQualifiedName, // 存储类的全名，方便以后反射加载
                Position = graphMousePosition,
                AnimSpeed = 1f,   // 默认值
                WeightSpeed = 5f  // 默认值
            };

            // 3. 创建 UI 节点并传入数据对象
            var node = new AnimGraphNode(type, nodeData); // 这样改！
            node.SetPosition(new Rect(graphMousePosition, Vector2.zero));
    
            _graphView.AddElement(node);

            return true;
        }
    }
}