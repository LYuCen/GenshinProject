using UnityEngine;

namespace mModel.ScriptableConfig
{
    [CreateAssetMenu(menuName = "Config/PlayerAvatarMask")]
    public class AvatarMaskConfig : ScriptableObject
    {
        public AvatarMask avatarMask;
    }
}