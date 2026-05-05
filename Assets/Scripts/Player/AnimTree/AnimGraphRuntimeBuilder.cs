using System;
using System.Collections.Generic;
using System.Linq;
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;

namespace Player.AnimTree
{
    public static class AnimGraphRuntimeBuilder
    {
        public static AnimTreeNode<PlayerManager> Build(AnimGraphData data, PlayerManager manager, PlayerAnimSlot slotContainer)
        {
            if (!data || data.Nodes.Count == 0) return null;

            // 1. 实例化所有节点并存储在字典中，Key 是 GUID
            Dictionary<string, AnimTreeNode<PlayerManager>> nodeInstances = new();

            foreach (var nodeData in data.Nodes)
            {
                // 通过反射创建逻辑类实例
                Type type = Type.GetType(nodeData.NodeType);
                if (type == null)
                {
                    Debug.LogError($"[AnimGraph] 找不到逻辑类类型: {nodeData.NodeType}");
                    continue;
                }

                // 调用构造函数 (PlayerManager manager)
                object[] args = new object[] { manager };
                var instance = (AnimTreeNode<PlayerManager>)Activator.CreateInstance(type, args);

                // 2. 注入编辑器中配置的参数
                instance.Tag = nodeData.Tag;
                instance.AnimSpeed = nodeData.AnimSpeed;
                instance.WeightSpeed = nodeData.WeightSpeed;
                instance.UseTagOnly = nodeData.useTagOnly;
                instance.DefaultClip = nodeData.DefaultClip;
                instance.Mask = nodeData.Mask;

                // 判断是否为插槽节点并判断是否有 Tag
                if (instance is AnimTreeSlotNode<PlayerManager> slotNode && !string.IsNullOrEmpty(instance.Tag))
                {
                    slotContainer.Register(instance.Tag, slotNode);
                }
                
                if (instance is AnimTreeLeafNode<PlayerManager> leafNode)
                {
                    leafNode.CurrentData = nodeData.weaponAsset; 
                }
                
                nodeInstances.Add(nodeData.GUID, instance);
            }

            HashSet<string> childGuids = new();
            
            foreach (var edge in data.Edges)
            {
                if (nodeInstances.TryGetValue(edge.ParentGUID, out var parent) &&
                    nodeInstances.TryGetValue(edge.ChildGUID, out var child))
                {
                    parent.AddNode(child);

                    childGuids.Add(edge.ChildGUID);
                }
            }

            // 3. 寻找根节点 (没被当过子节点的节点)
            string rootGuid = nodeInstances.Keys.FirstOrDefault(guid => !childGuids.Contains(guid));

            if (string.IsNullOrEmpty(rootGuid))
            {
                Debug.LogError("[AnimGraph] 无法识别根节点，请确保图中有且只有一个顶级节点");
                return null;
            }
            Debug.Log($"Builder: 成功找到根节点，类型: {nodeInstances[rootGuid].GetType().Name}");
            return nodeInstances[rootGuid];
        }
    }
}