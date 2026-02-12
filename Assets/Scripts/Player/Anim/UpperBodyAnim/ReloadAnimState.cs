using AssetBundleFramework.Core.Resource;
using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using ResourceManager = AssetBundleFramework.Core.Resource.ResourceManager;

namespace Player.Anim.UpperBodyAnim
{
    public class ReloadAnimState : PlayerAnimState
    {
        private AnimationClipPlayable _reloadPlayable;
        private bool _isReload;
        public ReloadAnimState(PlayerManager myManager) : base(myManager)
        {
            myManager.ReloadState = this;
        }

        protected override bool CheckState()
        {
            return MyManager.isReload;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            IResource ir =
                ResourceManager.instance.Load(
                    "Assets/AssetBundle/ScriptableObject/Anim/Weapon/FN_IWS/ReloadAnim.asset",
                    false);
            AnimConfig animConfig = ir.GetAsset<AnimConfig>();
            Clip = animConfig.animClip;
            _reloadPlayable = AnimationClipPlayable.Create(graph, Clip);
            _reloadPlayable.SetSpeed(animConfig.animSpeed);
            _reloadPlayable.Pause(); // 暂停动画
            return _reloadPlayable;
        }

        public override void OnExitState()
        {
            _isReload = false;
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeUpperWeight(this);
            
            // 每次切到 Reload 动画时，从头开始
            if (!_isReload)
            {
                _reloadPlayable.SetTime(0);
                _reloadPlayable.SetTime(0);
                _reloadPlayable.Play();
                _isReload = true;
            }
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }

        public override void SetAnimClip(AnimConfig config)
        {
            var graph = _reloadPlayable.GetGraph();
            // 创建一个新的动画节点
            AnimationClipPlayable anim = AnimationClipPlayable.Create(graph, config.animClip);
            anim.SetSpeed(config.animSpeed);
            anim.Pause();
            
            Parent.SetAnimClip(anim, MixIndex);
            _reloadPlayable = anim;
        }
    }
}