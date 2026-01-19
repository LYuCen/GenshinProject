using UnityEngine;

namespace mModel.ScriptableConfig
{
    [CreateAssetMenu(menuName = "Config/PlayerAnimConfig")]
    public class AnimConfig : ScriptableObject
    {
        public AnimationClip animClip;
        public float animSpeed;
    }
}