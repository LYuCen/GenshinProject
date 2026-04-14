using System.Collections.Generic;
using System.Linq;
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

        public AnimGraphView(EditorWindow window) // 传入窗口引用
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddSearchWindow(window);
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
            // 1. 清空当前画布
            DeleteElements(graphElements);

            // 2. 重新生成所有节点
            Dictionary<string, AnimGraphNode> nodeMap = new();
            foreach (var nData in dataAsset.Nodes)
            {
                // 这里需要通过反射根据 NodeType 字符串还原 Type
                System.Type type = System.Type.GetType(nData.NodeType);
                var node = new AnimGraphNode(type, nData);
                node.SetPosition(new Rect(nData.Position, Vector2.zero));
        
                AddElement(node);
                nodeMap.Add(nData.GUID, node);
            }

            // 3. 重新建立连线
            foreach (var eData in dataAsset.Edges)
            {
                var parent = nodeMap[eData.ParentGUID];
                var child = nodeMap[eData.ChildGUID];

                // 这里的 input 和 output 对应你在 AnimGraphNode 里定义的顺序
                var parentOutput = parent.outputContainer[0] as Port;
                var childInput = child.inputContainer[0] as Port;

                var edge = parentOutput.ConnectTo(childInput);
                AddElement(edge);
            }
        }
    }
}