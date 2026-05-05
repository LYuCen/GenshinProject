using Firearms;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player
{
    public class LayerMixerSample : MonoBehaviour
    {
        public Animator animator;
        public AnimationClip idleAnim;
        public AnimationClip walkAnim;
        public AnimationClip reloadAnim;
        public AvatarMask avatarMask;
        [Range(0f, 1f)] public float layerWeight;
        [Range(0f, 1f)] public float mixerWeight;
        
        private PlayableGraph _graph;
        private AnimationLayerMixerPlayable _layer;
        private AnimationMixerPlayable _mixer;
        
        // 变成类成员，这样 Update 才能用
        private AnimationClipPlayable _idlePlayable;
        private AnimationClipPlayable _walkPlayable;
        private AnimationClipPlayable _reloadPlayable;
        
        public WeaponSocketTool weaponSync;

        [Header("调试工具")]
        public bool debugMode = false;
        [Range(0f, 1f)] public float debugTimeNormalized;
        public KeyCode pauseKey = KeyCode.Space;
        private bool _isPaused = false;

        private void Start()
        {
            _graph = PlayableGraph.Create("layerGraph");
            animator.applyRootMotion = false;
            
            _mixer = AnimationMixerPlayable.Create(_graph, 2);
            _idlePlayable = AnimationClipPlayable.Create(_graph, idleAnim); // 赋值给成员
            _walkPlayable = AnimationClipPlayable.Create(_graph, walkAnim); // 赋值给成员
            
            _graph.Connect(_idlePlayable, 0, _mixer, 0);
            _graph.Connect(_walkPlayable, 0, _mixer, 1);
            
            _layer = AnimationLayerMixerPlayable.Create(_graph, 2);
            _reloadPlayable = AnimationClipPlayable.Create(_graph, reloadAnim);
            
            _graph.Connect(_mixer, 0, _layer, 0);
            _graph.Connect(_reloadPlayable, 0, _layer, 1);
            
            _layer.SetLayerMaskFromAvatarMask(1, avatarMask);
            _layer.SetInputWeight(0, 1);

            AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "output", animator);
            output.SetSourcePlayable(_layer);
            _graph.Play();
        }

        private void Update()
        {
            // 暂停逻辑
            if (Input.GetKeyDown(pauseKey))
            {
                _isPaused = !_isPaused;
                _layer.SetSpeed(_isPaused ? 0 : 1);
            }

            // 调试模式
            if (debugMode)
            {
                _reloadPlayable.SetSpeed(0);
                float targetTime = debugTimeNormalized * reloadAnim.length;
                _reloadPlayable.SetTime(targetTime);
            }
            
            // 设置混合权重
            _mixer.SetInputWeight(0, 1 - mixerWeight);
            _mixer.SetInputWeight(1, mixerWeight);
            _layer.SetInputWeight(1, layerWeight);

            // 每一帧都更新武器同步源
            UpdateWeaponSyncSource();
        }

        private void UpdateWeaponSyncSource()
        {
            if (weaponSync == null) return;

            // 逻辑修正：根据当前的权重，告诉同步脚本该看哪个动画
            if (layerWeight > 0.5f)
            {
                weaponSync.SetActiveAnimation(_reloadPlayable, reloadAnim.name);
            }
            else if (mixerWeight > 0.5f)
            {
                weaponSync.SetActiveAnimation(_walkPlayable, walkAnim.name);
            }
            else
            {
                weaponSync.SetActiveAnimation(_idlePlayable, idleAnim.name);
            }
        }

        private void OnDisable()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}