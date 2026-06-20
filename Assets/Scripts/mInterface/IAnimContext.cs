using UnityEngine;

namespace mInterface
{
    public interface IAnimContext
    {
        IMoveQueryProvider MoveModule { get; }
        ICombatQueryProvider CombatModule { get; }
        IIkProvider IkModule { get; }
        
        Transform WeaponPivot { get; }
        Animator Animator { get; }
        float GetAimIkTarget();
        float GetHandIkTarget();
    }
}