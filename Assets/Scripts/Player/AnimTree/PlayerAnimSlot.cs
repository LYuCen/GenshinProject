using System.Collections.Generic;
using mInterface;
using Player.AnimPro;

namespace Player.AnimTree
{
    public class PlayerAnimSlot
    {
        // 以 Tag 为键，存储所有的插槽节点
        private Dictionary<string, AnimTreeSlotNode<IAnimContext>> _slots = new();

        public void Register(string tag, AnimTreeSlotNode<IAnimContext> node)
        {
            if (string.IsNullOrEmpty(tag)) return;
            _slots[tag] = node;
        }

        public void Clear() => _slots.Clear();

        // 获取插槽的方法
        public AnimTreeSlotNode<IAnimContext> Get(string tag)
        {
            if (_slots.TryGetValue(tag, out var node))
            {
                return node;
            }
            return null;
        }

        // 方便外部检查
        public bool Contains(string tag) => _slots.ContainsKey(tag);
    }
}