namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Frost
{
    public class FrostReaction : ElementReaction
    {
        public FrostReaction()
        {
            Reactions.Add(new FrostFreeze());
            Reactions.Add(new FrostThaw());
            Reactions.Add(new FrostSuperconduction());
        }
    }
}