namespace gamedraft
{
    public abstract class LightInfantry : Unit, IHealable, ICloneableUnit
    {
        new const int UnitDescriptionId = 1;
        new const string UnitName = "LightI";
        new static readonly Type Type = typeof(LightInfantry);
        new const int Attack = 3;
        new const int Defence = 3;
        new const int MaxHP = 7;
        public LightInfantry() : base(UnitDescriptionId, UnitName, Type, Attack, Defence, MaxHP)
        {
        }
        public LightInfantry(ISpecialAbility spec) : base(UnitDescriptionId, spec.UnitName, Type, Attack, Defence, MaxHP, spec.SpecialAbilityType, spec.SpecialAbilityStrength, spec.SpecialAbilityRange)
        {
        }
        public abstract ICloneableUnit? Clone();
    }

    public abstract class HeavyInfantry : Unit, IHealable
    {
        new const int UnitDescriptionId = 2;
        new const string UnitName = "HeavyI";
        new static readonly Type Type = typeof(HeavyInfantry);
        new const int Attack = 7;
        new const int Defence = 7;
        new const int MaxHP = 7;
        public HeavyInfantry() : base(UnitDescriptionId, UnitName, Type, Attack, Defence, MaxHP)
        {
        }
        public HeavyInfantry(ISpecialAbility spec) : base(UnitDescriptionId, spec.UnitName, Type, Attack, Defence, MaxHP, spec.SpecialAbilityType, spec.SpecialAbilityStrength, spec.SpecialAbilityRange)
        {
        }
    }

    public abstract class Knight : Unit
    {
        new const int UnitDescriptionId = 3;
        new const string UnitName = "Knight";
        new static readonly Type Type = typeof(Knight);
        new const int Attack = 10;
        new const int Defence = 8;
        new const int MaxHP = 24;
        public Knight() : base(UnitDescriptionId, UnitName, Type, Attack, Defence, MaxHP)
        {
        }
        public Knight(ISpecialAbility spec) : base(UnitDescriptionId, spec.UnitName, Type, Attack, Defence, MaxHP, spec.SpecialAbilityType, spec.SpecialAbilityStrength, spec.SpecialAbilityRange)
        {
        }
    }
}