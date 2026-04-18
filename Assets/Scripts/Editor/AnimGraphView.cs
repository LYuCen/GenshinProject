using System;
using System.Collections.Generic;
using System.Linq;
using Player.AnimPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    // 在 AnimGraphView.cs 中修改构造函数和添加方法
    public class AnimGraphView : GraphView
    {
        private NodeSearchWindow _searchWindow;
        private AnimGraphData _currentAsset; // 增加字段记录当前资源
        
        public AnimGraphData CurrentAsset => _currentAsset;

        public AnimGraphView(EditorWindow window)
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
            
            AddSearchWindow(window);
            
            this.graphViewChanged = (changes) =>
            {
                if (_currentAsset != null && (changes.movedElements != null || changes.elementsToRemove != null || changes.edgesToCreate != null))
                {
                    Undo.RecordObject(_currentAsset, "Anim Tree Change");
                }
                return changes;
            };
            
            serializeGraphElements = (elements) =>
            {
                var selectedNodes = elements.OfType<AnimGraphNode>().Select(x => x.Data).ToList();
                if (selectedNodes.Count == 0) return string.Empty;

                return JsonUtility.ToJson(new CopyPasteData { nodes = selectedNodes });
            };

            // 2. 定义哪些数据可以被粘贴
            canPasteSerializedData = (data) =>
            {
                // 这里简单判断一下字符串是否包含我们的数据结构特征
                return data.Contains("NodeType") && data.Contains("GUID");
            };

            // 3. 定义粘贴时的具体逻辑
            unserializeAndPaste = (operationName, data) =>
            {
                if (string.IsNullOrEmpty(data)) return;

                var copyData = JsonUtility.FromJson<CopyPasteData>(data);
                foreach (var nodeData in copyData.nodes)
                {
                    // 生成新 GUID 并偏移位置
                    nodeData.GUID = Guid.NewGuid().ToString();
                    nodeData.Position += new Vector2(50, 50);

                    Type type = Type.GetType(nodeData.NodeType);
                    if (type == null) continue;

                    // 创建新节点
                    var newNode = new AnimGraphNode(type, nodeData, _currentAsset);
                    newNode.SetPosition(new Rect(nodeData.Position, Vector2.zero));
                    AddElement(newNode);
                }
            };
        }
        
        private void AddSearchWindow(EditorWindow window)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Init(this, window);

            // 按空格键或右键时呼出搜索窗口
            nodeCreationRequest = context => 
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        // 必须重写此方法，否则无法连线
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(port => 
                port.direction != startPort.direction && 
                port.node != startPort.node).ToList();
        }

        public void SaveGraph(AnimGraphData dataAsset)
        {
            if (dataAsset == null) return;

            // 1. 清空旧数据
            dataAsset.Nodes.Clear();
            dataAsset.Edges.Clear();

            // 2. 保存所有节点
            var nodes = this.nodes.ToList().Cast<AnimGraphNode>();
            foreach (var node in nodes)
            {
                // 记录节点在画布上的位置，方便下次打开
                node.Data.Position = node.GetPosition().position;
                dataAsset.Nodes.Add(node.Data);
            }

            // 3. 保存所有连线
            var edges = this.edges.ToList();
            foreach (var edge in edges)
            {
                var parent = edge.output.node as AnimGraphNode;
                var child = edge.input.node as AnimGraphNode;

                if (parent != null && child != null)
                {
                    dataAsset.Edges.Add(new EdgeData
                    {
                        ParentGUID = parent.Data.GUID,
                        ChildGUID = child.Data.GUID
                    });
                }
            }

            // 4. 强制 Unity 刷新文件
            EditorUtility.SetDirty(dataAsset);
            AssetDatabase.SaveAssets();
            Debug.Log($"[AnimGraph] 已成功保存到: {AssetDatabase.GetAssetPath(dataAsset)}");
        }
        
        public void LoadGraph(AnimGraphData dataAsset)
        {
            _currentAsset = dataAsset; 
            DeleteElements(graphElements);

            Dictionary<string, AnimGraphNode> nodeMap = new();
            foreach (var nData in dataAsset.Nodes)
            {
                System.Type type = System.Type.GetType(nData.NodeType);
                // 传入 _currentAsset
                var node = new AnimGraphNode(type, nData, _currentAsset); 
                node.SetPosition(new Rect(nData.Position, Vector2.zero));
                AddElement(node);
                nodeMap.Add(nData.GUID, node);
            }

            foreach (var eData in dataAsset.Edges)
            {
                if (nodeMap.TryGetValue(eData.ParentGUID, out var parent) && nodeMap.TryGetValue(eData.ChildGUID, out var child))
                {
                    var parentOutput = parent.outputContainer[0] as Port;
                    var childInput = child.inputContainer[0] as Port;
                    var edge = parentOutput.ConnectTo(childInput);
                    AddElement(edge);
                }
            }
        }

        [Serializable]
        public class CopyPasteData 
        { 
            public List<NodeData> nodes; 
        }
    }
}