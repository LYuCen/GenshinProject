using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff
{
    public class ElementComponent
    {
        public Transform Transform;
        public ElementBuffController Controller;
        public int Element;
        public bool IsStart; // 是否为刚挂上的元素
    }
}