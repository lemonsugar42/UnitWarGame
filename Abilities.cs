namespace gamedraft
{
    public class Archer : ISpecialAbility
    {
        public string UnitName { get; } = "Archer";
        public int SpecialAbilityType { get; } = 1;
        public int SpecialAbilityStrength { get; } = 3;
        public int SpecialAbilityRange { get; } = 3;

        public static string DoAction(int pos, Army my, Army enemy, IStrategy strategy)
        {
            return strategy.DoActionArcher(pos, my, enemy);
        }
    }

    public class Healer : ISpecialAbility
    {
        public string UnitName { get; } = "Healer";
        public int SpecialAbilityType { get; } = 2;
        public int SpecialAbilityStrength { get; } = 4;
        public int SpecialAbilityRange { get; } = 2;
        public static string DoAction(int pos, Army my, Army enemy, IStrategy strategy)
        {
            return strategy.DoActionHealer(pos, my, enemy);
        }
    }

    public class Witcher : ISpecialAbility
    {
        public string UnitName { get; } = "Wizard";
        public int SpecialAbilityType { get; } = 3;
        public int SpecialAbilityStrength { get; } = 3;
        public int SpecialAbilityRange { get; } = 3;
        public static string DoAction(int pos, Army my, Army enemy, IStrategy strategy)
        {
            return strategy.DoActionWitcher(pos, my, enemy);
        }
    }

}