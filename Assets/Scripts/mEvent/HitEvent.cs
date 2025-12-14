namespace mEvent
{
    public class HitEvent
    {
        public int HitType;
        public bool IsKill;
        
        public HitEvent(int hitType, bool isKill)
        {
            HitType = hitType;
            IsKill = isKill;
        }
    }
}