namespace gamedraft
{
    public interface IUnit
    {
        int UnitDescriptionId { get; }
        string UnitName { get; }
        string Name { get; }
        Type Type { get; }
        int Attack { get; }
        int Defence { get; }
        int MaxHP { get; }
        int HitPoints { get; set; }
        int UnitPrice { get; }
        string DoDamage(int attack, Unit unit);
        string TakeDamage(int attack, int price);
        public void Wipe();
    }
}
