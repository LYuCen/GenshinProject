using System;
using System.Collections.Generic;
using mInterface;
using Player.AnimPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.AnimGraph
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

            var types = TypeCache.GetTypesDerivedFrom(typeof(AnimTreeNode<IAnimContext>));

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

            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(
                _window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);

            var nodeData = new NodeData
            {
                GUID = Guid.NewGuid().ToString(),
                NodeType = type.AssemblyQualifiedName,
                Position = graphMousePosition,
                AnimSpeed = 1f,
                WeightSpeed = 5f
            };

            // --- 修正点：传入第三个参数 _graphView.CurrentAsset ---
            var node = new AnimGraphNode(type, nodeData, _graphView.CurrentAsset); 
            node.SetPosition(new Rect(graphMousePosition, Vector2.zero));

            _graphView.AddElement(node);
            return true;
        }
    }
}