using AssetBundleFramework.Core.Resource;
using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove.RifleSprint
{
    public class RifleLayerMixSprint : PlayerAnimState
    {
        private AnimConfig _configL;
        private AnimConfig _configR;
        private AnimationLayerMixerPlayable _rifleSprintLayer;
        private AvatarMask _avatarMask;
        
        public RifleLayerMixSprint(PlayerManager myManager) : base(myManager)
        {
            AddState(new RifleSprintMoveF(myManager));
            AddState(new RifleSprintMoveHeadL(myManager));
            AddState(new RifleSprintMoveHeadR(myManager));
        }

        protected override bool CheckState()
        {
            return MyManager.isSprint && !MyManager.isThrow;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            int inputCount = Children.Count;
            MixIndex = mixIndex;
            _rifleSprintLayer = AnimationLayerMixerPlayable.Create(graph, inputCount);
            IResource ir = ResourceManager.instance.Load("Assets/AssetBundle/AvatarMask/PlayerAvatarMask.mask", false);
            _avatarMask = ir.GetAsset<AvatarMask>();
            for (int i = 0; i < inputCount; i++)
            {
                Playable playable = Children[i].BuildNode(graph, i);
                graph.Connect(playable, 0, _rifleSprintLayer, i);
            }

            _rifleSprintLayer.SetInputWeight(0, 1f);
            _rifleSprintLayer.SetLayerMaskFromAvatarMask(1, _avatarMask);
            _rifleSprintLayer.SetLayerMaskFromAvatarMask(2, _avatarMask);

            return _rifleSprintLayer;
        }

        public override void OnExitState()
        {
            
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeLowerWeight(this);
            
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }
    }
}