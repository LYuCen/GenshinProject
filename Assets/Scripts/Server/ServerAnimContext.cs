using mInterface;
using UnityEngine;

namespace Server
{
    public class ServerAnimContext : IAnimContext
    {
        public IMoveQueryProvider MoveModule { get; }
        public ICombatQueryProvider CombatModule { get; }
        public IIkProvider IkModule { get; } = new ServerIkProvider();
        public Transform WeaponPivot { get; }
        public Animator Animator { get; }

        public ServerAnimContext(
            IMoveQueryProvider moveProvider,
            ICombatQueryProvider combatProvider,
            Transform weaponPivot,
            Animator animator)
        {
            MoveModule = moveProvider;
            CombatModule = combatProvider;
            WeaponPivot = weaponPivot;
            Animator = animator;
        }

        public float GetAimIkTarget() => 1f;
        public float GetHandIkTarget() => 1f;
    }
}
