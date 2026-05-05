
using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.CrouchLowerBodyNode
{
    /// <summary>
    /// 蹲待机
    /// </summary>
    public class CrouchIdleTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;

        // 修改后的构造函数：支持动态传入动画配置路径
        public CrouchIdleTreeLeafNode(PlayerManager myManager) : base(myManager)
        {
        }

        public override bool CheckState(bool parentActive)
        { 
            return parentActive && MyMessage.IsGround && MyMessage.InputMove == Vector2.zero;
        }

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