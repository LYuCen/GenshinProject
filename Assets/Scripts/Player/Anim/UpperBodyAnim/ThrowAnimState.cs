using Common.FSM.PlayerFSM;
using mModel.ScriptableConfig;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.Anim.UpperBodyAnim
{
    public class ThrowAnimState : PlayerAnimState
    {
        private AnimationClipPlayable _throwPlayable;
        public ThrowAnimState(PlayerManager myManager) : base(myManager)
        {
            
        }

        protected override bool CheckState()
        {
            return !MyManager.firearmController.isReadyThrow;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Config = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Weapon/Grenade/GrenadeUp.asset");
            Clip = Config.animClip;
            _throwPlayable = AnimationClipPlayable.Create(graph, Clip);
            _throwPlayable.SetSpeed(Config.animSpeed);
            _throwPlayable.Pause(); // 暂停动画
            return _throwPlayable;
        }

        public override void OnExitState()
        {
            
        }

        public override void NextState()
        {
            if (!CheckState()) return;
            MyManager.PlayAnimController.ChangeUpperWeight(this);
        }

        public override void SetWeight(int index, float weight = 0)
        {
            
        }
    }
}