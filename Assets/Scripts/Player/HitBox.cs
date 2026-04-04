using mEnum;
using mInterface;
using UnityEngine;

namespace Player
{
    public class HitBox : MonoBehaviour {
        public HitPart part;
        public IEntityIdentity Owner; // 这里的 owner 指向根节点的 PlayerManager

        void Awake() {
            // 自动向上寻找根节点的 Identity
            Owner = GetComponentInParent<IEntityIdentity>();
        }
    }
}