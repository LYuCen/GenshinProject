namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Fire
{
    public class FireReaction : ElementReaction
    {
        public FireReaction()
        {
            Reactions.Add(new FireEvaporation());
            Reactions.Add(new FireThaw());
            Reactions.Add(new FireOverLoad());
            Reactions.Add(new FireBurn());
        }
    }
}