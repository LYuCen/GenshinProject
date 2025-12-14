using Common.FSM.PlayerFSM;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.LowerBodyAnim.BaseAnim.BaseMove.GrenadeSprint
{
    public class GrenadeLayerMixSprint : PlayerAnimState
    {
        private PlayerAnimConfig _configL;
        private PlayerAnimConfig _configR;
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
            _avatarMask = Resources.Load<PlayerAvatarMask>("Anim/Rifle/Sprint/HeadMask").avatarMask;
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
            float angle = MyManager.cameraController.GetAngle();
            if (angle > 1)
            {
                if (angle > 90) angle = 180 - angle;
                _grenadeSprintLayer.SetInputWeight(1, 0f);
                float weight = Mathf.Clamp(angle / 90f, 0, 1f);
                _grenadeSprintLayer.SetInputWeight(2, weight);
            }
            else if(angle < -1)
            {
                angle = -angle;
                if (angle > 90) angle = 180 - angle;
                _grenadeSprintLayer.SetInputWeight(2, 0f);
                float weight = Mathf.Clamp(angle / 90f, 0, 1f);
                _grenadeSprintLayer.SetInputWeight(1, weight);
            }
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }
    }
}