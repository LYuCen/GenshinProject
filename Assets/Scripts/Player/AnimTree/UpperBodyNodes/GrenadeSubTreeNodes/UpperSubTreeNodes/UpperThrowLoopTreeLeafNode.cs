using Firearms;
using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public class UpperThrowLoopTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AbstractThrow _throw;
        private AnimationClipPlayable _playable;
        public UpperThrowLoopTreeLeafNode(PlayerManager myMessage, AbstractThrow s) : base(myMessage)
        {
            _throw = s;
        }

        public override bool CheckState(bool parentActive)
        {
            return parentActive && _throw.IsReadyThrow;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Weapon/Grenade/Throw/ThrowHighLoop.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            return _playable;
        }
    }
}