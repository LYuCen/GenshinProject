using System.Collections.Generic;
using BaseClass;

namespace mSystem
{
    public class ComponentSystem<T> where T : class
    {
        protected Queue<T> Queue = new();

        public virtual void EnQueue(T component)
        {
            Queue.Enqueue(component);
        }

        public void FixedUpdate()
        {
            UpdateComponent();
        }

        protected virtual void UpdateComponent(){}
    }
}