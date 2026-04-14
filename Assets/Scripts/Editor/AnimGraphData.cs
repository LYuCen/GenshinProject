using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    // 这个特性让你可以在 Project 面板右键创建这个资源文件
    [CreateAssetMenu(menuName = "AnimPro/AnimGraphData")]
    public class AnimGraphData : ScriptableObject
    {
        public List<NodeData> Nodes = new();
        public List<EdgeData> Edges = new();
    }

    [System.Serializable]
    public class EdgeData
    {
        public string ParentGUID;
        public string ChildGUID;
    }
    
    [System.Serializable]
    public class NodeData
    {
        public string GUID;
        public string NodeType;
        public Vector2 Position;

        // --- 资源配置 ---
        public bool useTagOnly;           // 是否仅通过 Tag 匹配（忽略默认 Clip）
        public string Tag;                // 匹配标签（如 "Reload"）
        public AnimationClip DefaultClip; // 直接拖入的默认动画

        public float AnimSpeed = 1f; 
        public float WeightSpeed = 5f;
    }
}