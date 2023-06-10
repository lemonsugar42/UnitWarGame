namespace gamedraft
{
    public interface ISpecialAbility
    {
        string UnitName { get; }
        int SpecialAbilityType { get; }
        int SpecialAbilityStrength { get; }
        int SpecialAbilityRange { get; }
        static void DoAction(int pos, Army one, Army other, IStrategy strategy) { }
    }
}
