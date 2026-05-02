using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using Common.LevelFSM.EnemyFSM;
using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Playables;

namespace Player.Anim
{
    public abstract class AnimState<T>
    {
        protected List<AnimState<T>> Children = new();
        protected T MyManager;
        public AnimState<T> Parent;
        protected AnimationClip Clip = null;
        protected AnimConfig Config;
        protected bool IsLeaf;
        protected float Weight;
        protected float targetWeight;
        public int MixIndex;

        protected AnimState(T myManager)
        {
            MyManager = myManager;
        }

        /// <summary>
        /// 添加子状态
        /// </summary>
        /// <param name="state">子状态</param>
        protected void AddState(AnimState<T> state)
        {
            Children.Add(state);
        }

        /// <summary>
        /// 检查状态
        /// </summary>
        /// <returns>如果为 true 将切换该状态</returns>
        protected abstract bool CheckState();
        
        /// <summary>
        /// 设置权重（mix）
        /// </summary>
        /// <param name="index">切换权重下标</param>
        /// <param name="weight">权值</param>
        public abstract void SetWeight(int index, float weight = 0);
        
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="graph">动画树</param>
        /// <param name="mixIndex">混合下标</param>
        /// <returns>返回状态节点</returns>
        public abstract Playable BuildNode(PlayableGraph graph, int mixIndex = -1);

        /// <summary>
        /// 切换动画节点
        /// </summary>
        /// <param name="playable">新动画节点</param>
        /// <param name="index">动画下标</param>
        public virtual void SetAnimClip(Playable playable, int index){}
        
        /// <summary>
        /// 切换动画节点，外界调用的方法
        /// </summary>
        /// <param name="config">动画信息</param>
        public virtual void SetAnimClip(AnimConfig config){}
        
        /// <summary>
        /// 进入当前状态
        /// </summary>
        public virtual void OnExitState(){}
        
        /// <summary>
        /// 离开当前状态
        /// </summary>
        public virtual void OnEnterState(){}

        /// <summary>
        /// 获取动画文件
        /// </summary>
        /// <param name="animPath">AssetBundle 文件目录</param>
        protected virtual AnimConfig GetAnimConfig(string animPath)
        {
            IResource ir =
                ResourceManager.instance.Load(
                    animPath,
                    false);
            return ir.GetAsset<AnimConfig>();
        }

        /// <summary>
        /// 检查状态
        /// </summary>
        public virtual void NextState()
        {
            if (!CheckState()) return;

            if (Children.Count > 0)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].NextState();
                }
            }
        }

        public virtual float UpdateNode()
        {
            if (IsLeaf)
            {
                Weight = Mathf.MoveTowards(Weight, targetWeight, Time.fixedDeltaTime);
                return Weight;
            }

            float maxTarget = 0f;

            foreach (var child in Children)
            {
                child.UpdateNode();
                maxTarget = Mathf.Max(maxTarget, child.targetWeight);
            }

            targetWeight = maxTarget;
            Weight = Mathf.MoveTowards(Weight, targetWeight, Time.fixedDeltaTime);

            return Weight;
        }
    }

    public abstract class PlayerAnimState : AnimState<PlayerManager>
    {
        public new PlayerAnimState Parent
        {
            get => base.Parent as PlayerAnimState;
            set => base.Parent = value;
        }

        protected PlayerAnimState(PlayerManager myManager) : base(myManager)
        {
        }
    }

    public abstract class EnemyAnimState : AnimState<EnemyManager>
    {
        public new EnemyAnimState Parent
        {
            get => base.Parent as EnemyAnimState;
            set => base.Parent = value;
        }

        protected EnemyAnimState(EnemyManager myManager) : base(myManager)
        {
            
        }
    }
}