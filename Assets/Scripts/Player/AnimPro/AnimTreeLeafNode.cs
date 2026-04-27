using Firearms;

namespace Player.AnimPro
{
    public abstract class AnimTreeLeafNode<T> : AnimTreeNode<T>
    {
        public WeaponClipAsset CurrentData;
        
        protected AnimTreeLeafNode(T myMessage) : base(myMessage)
        {
            IsLeaf = true;
        }
    }
}