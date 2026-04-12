using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree.UpperBodyNodes
{
    /// <summary>
    /// 换弹节点
    /// </summary>
    public class ReloadTreeLeafNode : AnimTreeLeafNode<PlayerManager>
    {
        private AnimationClipPlayable _playable;
        private float _clipDuration;

        public ReloadTreeLeafNode(PlayerManager manager) : base(manager) { }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.IsReload;

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Weapon/Gun/FN_IWS/ReloadAnim.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            _clipDuration = cfg.animClip.length;
            return _playable;
        }

        public override void OnEnterState()
        {
            Debug.Log("Start Reloading...");
            _playable.SetTime(0); // 每次进入时重置时间
        }

        public override float UpdateNode()
        {
            float currentWeight = base.UpdateNode();

            // 逻辑：如果当前权重较大（已切入），且动画播放完毕，则重置状态
            if (currentWeight > 0.5f && MyMessage.IsReload)
            {
                if (_playable.GetTime() >= _clipDuration)
                {
                    Debug.Log("Reload Finished.");
                    MyMessage.ReloadEnd();
                }
            }
            return currentWeight;
        }

        public override void OnExitState() => Debug.Log("Exit Reloading");
    }
}