using mSystem.StatusSystem;

namespace mInterface
{
    public interface IEntityIdentity {
        int EntityId { get; }

        EntityStatusUI EntityStatusUI { get; }
    }
}