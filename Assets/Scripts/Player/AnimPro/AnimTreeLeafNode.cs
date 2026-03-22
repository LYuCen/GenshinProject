namespace Player.AnimPro
{
    public abstract class AnimTreeLeafNode<T> : AnimTreeNode<T>
    {
        protected AnimTreeLeafNode(T myManager) : base(myManager)
        {
            IsLeaf = true;
        }
    }
}