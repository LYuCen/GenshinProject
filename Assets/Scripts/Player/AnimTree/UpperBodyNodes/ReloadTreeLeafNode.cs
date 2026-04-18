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

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            AnimationClip clipToUse = null;

            // 根据编辑器里的配置决定使用哪个 Clip
            if (UseTagOnly)
            {
                // 严格 Tag 模式：去当前武器里找
                if (MyMessage.firearmController.Weapon != null && !string.IsNullOrEmpty(Tag))
                {
                    clipToUse = MyMessage.firearmController.Weapon.GetClipByTag(Tag);
                }
            }
            else
            {
                // 混合模式：优先用图中拖进去的，没拖再用 Tag 找
                clipToUse = DefaultClip;
                if (!clipToUse && !string.IsNullOrEmpty(Tag))
                {
                    clipToUse = MyMessage.firearmController.Weapon?.GetClipByTag(Tag);
                }
            }

            if (!clipToUse)
            {
                Debug.LogError($"[ReloadNode] 找不到有效的动画剪辑！Tag: {Tag}");
                return Playable.Null;
            }
            
            _playable = AnimationClipPlayable.Create(graph, clipToUse);
            
            _playable.SetSpeed(AnimSpeed);
            
            _clipDuration = clipToUse.length;

            return _playable;
        }

        public override void OnEnterState()
        {
            Debug.Log("Start Reloading...");
            _playable.SetTime(0); // 每次进入时重置时间
        }
        
        public override bool CheckState(bool parentActive)
        {
            return (parentActive && MyMessage.IsReload) || Weight > 0.001f;
        }

        public override float UpdateNode()
        {
            TargetWeight = MyMessage.IsReload ? 1f : 0f;
            
            float currentWeight = base.UpdateNode();
            if (currentWeight > 0.9f && MyMessage.IsReload)
            {
                if (_playable.GetTime() >= _clipDuration)
                {
                    MyMessage.ReloadEnd(); // 这里会让 IsReload 变 false，触发上面的 TargetWeight = 0
                }
            }
            return currentWeight;
        }

        public override void OnExitState() => Debug.Log("Exit Reloading");
    }
}