using System.Text.Json.Serialization;

namespace gamedraft
{
    public class Unit : IUnit, ISpecialAbility
    {
        public int UnitDescriptionId { get; }
        public string UnitName { get; }
        [JsonIgnore]
        public string Name { get; protected set; }
        [JsonIgnore]
        public Type Type { get; protected set; }
        public int Attack { get; }
        public int Defence { get; }
        [JsonIgnore]
        public int MaxHP { get; }
        public int HitPoints { get; set; }
        [JsonIgnore]
        public int UnitPrice { get; protected set; }
        public int SpecialAbilityType { get; }
        public int SpecialAbilityStrength { get; }
        public int SpecialAbilityRange { get; }
        public Unit(int uid, string uname, Type type, int att, int def, int hp, int sat = 0, int sas = 0, int sar = 0)
        {
            UnitDescriptionId = uid;
            UnitName = uname;
            Name = new Random().NextInt64(1000, 1999).ToString();
            Type = type;
            Attack = att;
            Defence = def;
            HitPoints = MaxHP = hp;
            UnitPrice = att + def + hp;
            SpecialAbilityType = sat;
            SpecialAbilityStrength = sas;
            SpecialAbilityRange = sar;
            UnitPrice = att + def + hp + (sas + sar) * 2;
        }

        public string DoDamage(int price, Unit enemy)
        {
            LogProxy en = new LogProxy(enemy);
            return en.TakeDamage(this.Attack, price);
        }

        public virtual string TakeDamage(int attack, int price)
        {
            int damage = (int)Math.Ceiling(attack * (price - this.Defence) / 100.0d);
            this.HitPoints -= damage;
            return damage.ToString();
        }

        public string DoAction(int pos, Army one, Army other, IStrategy strategy)
        {
            string msg = "";
            if (this.SpecialAbilityType == 1)
            {
                msg = Archer.DoAction(pos, one, other, strategy);
            }
            if (this.SpecialAbilityType == 2)
            {
                msg = Healer.DoAction(pos, one, other, strategy);
            }
            if (this.SpecialAbilityType == 3)
            {
                msg = Witcher.DoAction(pos, one, other, strategy);
            }
            return msg;
        }

        //public static List<Unit> operator *(Unit unit, int amount)
        //{
        //    List<Unit> res = new List<Unit>();
        //    for (int i = 0; i < amount; i++)
        //    {
        //        res.Add(new Unit(unit.UnitDescriptionId, unit.UnitName, unit.Type, unit.Attack, unit.Defence, unit.HitPoints,
        //            unit.SpecialAbilityType, unit.SpecialAbilityStrength, unit.SpecialAbilityRange));
        //    }
        //    return res;
        //}

        public void Wipe() { }

        public object? Copy()
        {
            return this.MemberwiseClone();
        }
    }
}
