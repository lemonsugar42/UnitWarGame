namespace gamedraft
{

    public class SimpleLInf : LightInfantry
    {
        public SimpleLInf() : base() { }
        public SimpleLInf(SimpleLInf simple) : base()
        {
            Name = simple.Name + "_C";
            HitPoints = MaxHP;
        }
        public override ICloneableUnit? Clone()
        {
            if (new Random().Next(1, 30) <= SpecialAbilityStrength) return new SimpleLInf(this);
            else return null;
        }
    }
    public class ArcherLInf : LightInfantry
    {
        public ArcherLInf() : base(new Archer()) { }
        public ArcherLInf(ArcherLInf archer) : base(new Archer())
        {
            Name = archer.Name + "_C";
            HitPoints = MaxHP;
        }
        public override ICloneableUnit? Clone()
        {
            if (new Random().Next(1, 30) <= SpecialAbilityStrength) return new ArcherLInf(this);
            else return null;
        }
    }
    public class HealerLInf : LightInfantry
    {
        public HealerLInf() : base(new Healer()) { }
        public HealerLInf(HealerLInf healer) : base(new Healer())
        {
            Name = healer.Name + "_C";
            HitPoints = MaxHP;
        }
        public override ICloneableUnit? Clone()
        {
            if (new Random().Next(1, 30) <= SpecialAbilityStrength) return new HealerLInf(this);
            else return null;
        }
    }
    public class WitcherLInf : LightInfantry
    {
        public WitcherLInf() : base(new Witcher()) { }
        public WitcherLInf(WitcherLInf witcher) : base(new Witcher())
        {
            Name = witcher.Name + "_C";
            HitPoints = MaxHP;
        }
        public override ICloneableUnit? Clone()
        {
            if (new Random().Next(1, 30) <= SpecialAbilityStrength) return new WitcherLInf(this);
            else return null;
        }
    }


    public class SimpleHInf : HeavyInfantry { }
    public class ArcherHInf : HeavyInfantry
    {
        public ArcherHInf() : base(new Archer()) { }
    }
    public class HealerHInf : HeavyInfantry
    {
        public HealerHInf() : base(new Healer()) { }
    }
    public class WitcherHInf : HeavyInfantry
    {
        public WitcherHInf() : base(new Witcher()) { }
    }


    public class SimpleKnight : Knight { }
    public class ArcherKnight : Knight
    {
        public ArcherKnight() : base(new Archer()) { }
    }
    public class HealerKnight : Knight
    {
        public HealerKnight() : base(new Healer()) { }
    }
    public class WitcherKnight : Knight
    {
        public WitcherKnight() : base(new Witcher()) { }
    }

    
    public class SimpleGorod : GorodAdapter
    {
        public SimpleGorod(GulyayGorod gorod) : base(gorod) { }
    }
    public class ArcherGorod : GorodAdapter
    {
        public ArcherGorod(GulyayGorod gorod) : base(gorod, new Archer())
        {
        }
    }
    public class HealerGorod : GorodAdapter
    {
        public HealerGorod(GulyayGorod gorod) : base(gorod, new Healer())
        {
        }
    }
    public class WitcherGorod : GorodAdapter
    {
        public WitcherGorod(GulyayGorod gorod) : base(gorod, new Witcher())
        {
        }
    }


    public interface UnitFactory
    {
        LightInfantry CreateLight();
        HeavyInfantry CreateHeavy();
        Knight CreateKnight();
        GorodAdapter CreateGorod(GulyayGorod gorod);
    }


    public class SimpleFactory : UnitFactory
    {
        public LightInfantry CreateLight()
        {
            return new SimpleLInf();
        }
        public HeavyInfantry CreateHeavy()
        {
            return new SimpleHInf();
        }
        public Knight CreateKnight()
        {
            return new SimpleKnight();
        }
        public GorodAdapter CreateGorod(GulyayGorod gorod)
        {
            return new SimpleGorod(gorod);
        }
    }
    public class ArcherFactory : UnitFactory
    {
        public LightInfantry CreateLight()
        {
            return new ArcherLInf();
        }
        public HeavyInfantry CreateHeavy()
        {
            return new ArcherHInf();
        }
        public Knight CreateKnight()
        {
            return new ArcherKnight();
        }
        public GorodAdapter CreateGorod(GulyayGorod gorod)
        {
            return new ArcherGorod(gorod);
        }
    }
    public class HealerFactory : UnitFactory
    {
        public LightInfantry CreateLight()
        {
            return new HealerLInf();
        }
        public HeavyInfantry CreateHeavy()
        {
            return new HealerHInf();
        }
        public Knight CreateKnight()
        {
            return new HealerKnight();
        }
        public GorodAdapter CreateGorod(GulyayGorod gorod)
        {
            return new HealerGorod(gorod);
        }
    }
    public class WitcherFactory : UnitFactory
    {
        public LightInfantry CreateLight()
        {
            return new WitcherLInf();
        }
        public HeavyInfantry CreateHeavy()
        {
            return new WitcherHInf();
        }
        public Knight CreateKnight()
        {
            return new WitcherKnight();
        }
        public GorodAdapter CreateGorod(GulyayGorod gorod)
        {
            return new WitcherGorod(gorod);
        }
    }
}