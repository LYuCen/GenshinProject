using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes
{
    /// <summary>
    /// 持枪移动节点
    /// </summary>
    public class WalkTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;

        // 修改后的构造函数：支持动态传入动画配置路径
        public WalkTreeLeafNode(PlayerManager myManager) : base(myManager) { }

        public override bool CheckState(bool parentActive)
        {
            return parentActive && !MyMessage.IsShoot && !MyMessage.IsSprint;
        }

        // 修改后的 BuildNode：根据传入路径加载配置
        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            // 直接使用基类里被解析器注入的变量
            AnimationClip clipToUse = UseTagOnly ? null : DefaultClip;

            if (!clipToUse && !string.IsNullOrEmpty(Tag))
            {
                // 去武器表里找
                clipToUse = MyMessage.firearmController.Weapon.GetClipByTag(Tag);
            }

            var playable = AnimationClipPlayable.Create(graph, clipToUse);
            playable.SetSpeed(this.AnimSpeed); // 使用基类的 AnimSpeed
            return playable;
        }
    }
}