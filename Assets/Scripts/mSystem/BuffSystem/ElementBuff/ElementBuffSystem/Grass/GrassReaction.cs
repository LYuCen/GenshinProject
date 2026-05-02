namespace mSystem.BuffSystem.ElementBuff.ElementBuffSystem.Grass
{
    public class GrassReaction : ElementReaction
    {
        public GrassReaction()
        {
            Reactions.Add(new GrassBurn());
            Reactions.Add(new GrassSharpen());
        }
    }
}