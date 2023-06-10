namespace gamedraft
{
    public class GulyayGorod
    {
        private readonly int _health;
        private readonly int _defence;
        private readonly int _cost;
        private int _currentHealth;
        public GulyayGorod(int health, int defence, int cost)
        {
            _defence = defence;
            _health = _currentHealth = health;
            _cost = cost;
        }
        public int GetDefence()
        {
            return _defence;
        }
        public int GetStrength()
        {
            return 0;
        }
        public int GetHealth()
        {
            return _health;
        }
        public int GetCurrentHealth()
        {
            return _currentHealth;
        }
        public int GetCost()
        {
            return _cost;
        }
        public void TakeDamage(int damage)
        {
            if (_currentHealth == 0)
                throw new Exception("Unit are death!");

            if (damage < 0)
                throw new ArgumentException("Argument must be greater than zero", "damage");

            _currentHealth -= Math.Max(damage - _defence, 0);

            if (_currentHealth < 0)
                _currentHealth = 0;
        }
        public bool IsDead
        {
            get { return _currentHealth <= 0; }
        }
    }

    public abstract class GorodAdapter : Unit
    {
        private GulyayGorod gorod;
        new const int UnitDescriptionId = 4;
        new const string UnitName = "Gorod";
        new static readonly Type Type = typeof(GorodAdapter);
        public GorodAdapter(GulyayGorod gorod) : base(UnitDescriptionId, UnitName, Type, gorod.GetStrength(), gorod.GetDefence(), gorod.GetHealth())
        {
            this.gorod = gorod;
            UnitPrice = gorod.GetCost();
        }
        public GorodAdapter(GulyayGorod gorod, ISpecialAbility spec) : base(UnitDescriptionId, spec.UnitName, Type, gorod.GetStrength(), gorod.GetDefence(), gorod.GetHealth(), spec.SpecialAbilityType, spec.SpecialAbilityStrength, spec.SpecialAbilityRange)
        {
            this.gorod = gorod;
            UnitPrice = gorod.GetCost();
        }
        public override string TakeDamage(int attack, int price)
        {
            int before = HitPoints;
            gorod.TakeDamage(attack);
            int after = HitPoints;
            return (before - after).ToString();
        }
    }
}
