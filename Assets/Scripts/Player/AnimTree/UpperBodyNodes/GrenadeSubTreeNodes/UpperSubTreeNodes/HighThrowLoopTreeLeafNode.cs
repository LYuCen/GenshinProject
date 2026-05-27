using Firearms.ThrowWeapon;

using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public class HighThrowLoopTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        public HighThrowLoopTreeLeafNode(PlayerManager myMessage) : base(myMessage)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            return parentActive;
            //var grenade = MyMessage.firearmController.Weapon as Grenade;
            //return parentActive && grenade != null && grenade.IsReady;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            _playable = AnimationClipPlayable.Create(graph, DefaultClip);
            _playable.SetSpeed(this.AnimSpeed); // 使用基类的 AnimSpeed
            return _playable;
        }
    }
}