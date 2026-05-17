using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Playables;

namespace Player.AnimPro
{
    public abstract class AnimTreeNode<T>
    {
        public float Weight;            // 权重  
        public float WeightSpeed = 5f;  // 权重转移速度
        public float TargetWeight;      // 目标权重
        public int MixIndex;            // 在父节点下所处下标       
        protected List<AnimTreeNode<T>> Children = new(); 
        protected T MyMessage;                 
        public AnimTreeNode<T> Parent;
        protected AnimTreeNode<T> LastActiveChild; 
        
        
        public string Tag;                // 对应编辑器里的 Lookup Tag
        public float AnimSpeed = 1f;      // 对应编辑器里的 Anim Speed (主要用于叶子节点)
        public bool UseTagOnly;           // 对应编辑器里的 Use Tag Only 开关
        public AnimationClip DefaultClip; // 对应编辑器里的 Default Clip 拖拽框
        public AvatarMask Mask;           // 对应编辑器的 mask

        public AnimTreeNode(T myMessage) => MyMessage = myMessage;

        public virtual void AddNode(AnimTreeNode<T> node)
        {
            if (node == null) return;
            node.Parent = this;
            Children.Add(node);
        }

        /// <summary>
        /// 是否需要继续维持该节点的 Pose 更新。
        /// </summary>
        /// <param name="keepPoseAlive">
        /// 父节点是否要求该节点继续输出姿态。
        /// </param>
        public abstract void Evaluate(bool keepPoseAlive = true);

        /// <summary>
        /// 更新节点权重
        /// </summary>
        /// <returns>返回当前权重</returns>
        public abstract float UpdateNode();

        /// <summary>
        /// 检查节点状态
        /// </summary>
        /// <param name="parentActive">父节点是否激活</param>
        /// <returns>该节点条件是否满足</returns>
        public abstract bool CheckState(bool parentActive);
        
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="graph">PlayableGraph</param>
        /// <param name="mixIndex">当前节点所处下标</param>
        /// <returns>Playable Api节点</returns>
        public abstract Playable BuildNode(PlayableGraph graph, int mixIndex = -1);
        
        /// <summary>
        /// 进入该节点时调用
        /// </summary>
        public virtual void OnEnterState() { }
        
        /// <summary>
        /// 离开该节点后调用
        /// </summary>
        public virtual void OnExitState() { }
        
        /// <summary>
        /// 获取该下标节点当前的权重
        /// </summary>
        /// <param name="index">节点下标</param>
        /// <returns>节点权重</returns>
        protected virtual float GetInputWeight(int index) => 0f;
        
        /// <summary>
        /// 更新节点权重
        /// </summary>
        /// <param name="index">节点下标</param>
        /// <param name="w">权重值</param>
        protected virtual void SetInputWeight(int index, float w) { }

        /// <summary>
        /// 获取动画配置文件
        /// </summary>
        /// <param name="animPath">文件地址</param>
        /// <returns>配置文件 Config</returns>
        protected virtual AnimConfig GetAnimConfig(string animPath)
        {
            IResource ir = ResourceManager.instance.Load(animPath, false);
            return ir.GetAsset<AnimConfig>();
        }
    }
}