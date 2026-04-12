namespace Player.AnimPro
{
    public abstract class AnimTreeLeafNode<T> : AnimTreeNode<T>
    {
        protected AnimTreeLeafNode(T myMessage) : base(myMessage)
        {
            IsLeaf = true;
        }
    }
}