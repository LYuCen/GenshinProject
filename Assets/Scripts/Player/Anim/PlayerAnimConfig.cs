using UnityEngine;

namespace Player.Anim
{
    [CreateAssetMenu(menuName = "Config/PlayerAnimConfig")]
    public class PlayerAnimConfig : ScriptableObject
    {
        public AnimationClip animClip;
        public float animSpeed;
    }

    
}