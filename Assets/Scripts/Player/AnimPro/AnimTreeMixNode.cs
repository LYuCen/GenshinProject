using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimPro
{
    public abstract class AnimTreeMixNode<T> : AnimTreeNode<T>
    {
        protected AnimationMixerPlayable Mixer;

        public AnimTreeMixNode(T myManager) : base(myManager)
        {
            IsLeaf = false;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            MixIndex = mixIndex;
            Mixer = AnimationMixerPlayable.Create(graph, Children.Count);
            for (int i = 0; i < Children.Count; i++)
            {
                Playable childPlayable = Children[i].BuildNode(graph, i);
                graph.Connect(childPlayable, 0, Mixer, i);
                Mixer.SetInputWeight(i, 0f);
            }
            return Mixer;
        }

        protected override float GetInputWeight(int index) => Mixer.IsValid() ? Mixer.GetInputWeight(index) : 0f;
        
        protected override void SetInputWeight(int index, float w)
        {
            if (Mixer.IsValid()) Mixer.SetInputWeight(index, w);
        }

        public override void OnExitState()
        {
            // 1. 清理状态保持引用，防止下次进入时产生姿态跳变
            LastActiveChild = null;

            // 2. 强制清理所有子节点
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Weight > 0)
                {
                    Children[i].OnExitState();
                    Children[i].Weight = 0;
                    SetInputWeight(i, 0);
                }
            }
        }
    }
}