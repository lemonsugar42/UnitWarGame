namespace gamedraft
{
    public interface IHealable
    {
        public static void GainHealth(int hpower, Unit unit)
        {
            unit.HitPoints = Math.Min(unit.MaxHP, unit.HitPoints + hpower);
        }
    }
}
