namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Elect
{
    public class ElectReaction : ElementReaction
    {
        public ElectReaction()
        {
            Reactions.Add(new ElectOverLoad());
            Reactions.Add(new ElectSuperconduction());
            Reactions.Add(new ElectElectrification());
            Reactions.Add(new ElectSharpen());
        }
    }
}