using AssetBundleFramework.Core.Resource;
using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove.GrenadeSprint
{
    public class GrenadeLayerMixSprint : PlayerAnimState
    {
        private AnimConfig _configL;
        private AnimConfig _configR;
        private AnimationLayerMixerPlayable _grenadeSprintLayer;
        private AvatarMask _avatarMask;
        
        public GrenadeLayerMixSprint(PlayerManager myManager) : base(myManager)
        {
            AddState(new GrenadeSprintMoveF(myManager));
            AddState(new GrenadeSprintMoveHeadL(myManager));
            AddState(new GrenadeSprintMoveHeadR(myManager));
        }
        
        protected override bool CheckState()
        {
            return MyManager.isRun && MyManager.isThrow;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            int inputCount = Children.Count;
            MixIndex = mixIndex;
            _grenadeSprintLayer = AnimationLayerMixerPlayable.Create(graph, inputCount);
            IResource ir = ResourceManager.instance.Load("Assets/AssetBundle/AvatarMask/HeadMask.mask", false);
            _avatarMask = ir.GetAsset<AvatarMask>();
            for (int i = 0; i < inputCount; i++)
            {
                Playable playable = Children[i].BuildNode(graph, i);
                graph.Connect(playable, 0, _grenadeSprintLayer, i);
            }

            _grenadeSprintLayer.SetInputWeight(0, 1f);
            _grenadeSprintLayer.SetLayerMaskFromAvatarMask(1, _avatarMask);
            _grenadeSprintLayer.SetLayerMaskFromAvatarMask(2, _avatarMask);

            return _grenadeSprintLayer;
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