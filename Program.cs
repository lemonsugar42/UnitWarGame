using System.Text.Json.Serialization;
using System.Text.Json;

namespace gamedraft
{
    public class Game
    {
        private int state;
        private IStrategy strategy;
        private Army afirst, asecond;
        public bool Over { get; private set; }
        public Game(Army user, Army computer, int state = 0)
        {
            this.state = state;
            strategy = new OneOnOne();
            afirst = user;
            asecond = computer;
            Over = false;
        }

        public void SetStrategy(IStrategy strategy)
        {
            this.strategy = strategy;
        }
        public void RegroupUnits()
        {
            afirst.List.Where((u) => u == null).ToList().ForEach((u) => afirst.List.Remove(u));
            asecond.List.Where((u) => u == null).ToList().ForEach((u) => asecond.List.Remove(u));
        }

        public void ShowStats()
        {
            Console.WriteLine($"\n{afirst.Name} ({afirst.Price}) --- vs --- {asecond.Name} ({asecond.Price})");
            Console.WriteLine($"СОСТОЯНИЕ: {state}\tСТРАТЕГИЯ: {strategy.GetType().Name}\n");

            List<Unit?> one = afirst.List.Select(x => (Unit?)x).ToList();
            List<Unit?> two = asecond.List.Select(x => (Unit?)x).ToList();
            strategy.ShowStats(one, two);
        }
        public void ExchangeAttacks()
        {
            int lines = 0;
            switch(strategy)
            {
                case OneOnOne:
                    lines = 1;
                    break;
                case ThreeOnThree:
                    lines = 3;
                    break;
                case WallToWall:
                    lines = Math.Min(afirst.List.Count, asecond.List.Count);
                    break;
            }

            Unit? ufirst, usecond;
            for (int i = 0; i < lines; i++)
            {
                if ( i > (afirst.List.Count - 1) || i > (asecond.List.Count - 1) )
                {
                    continue;
                }
                ufirst = (Unit?)afirst.List[i];
                usecond = (Unit?)asecond.List[i];
                if ( ufirst == null || usecond == null)
                {
                    continue;
                }

                if (ufirst is not GorodAdapter)
                {
                    new LogProxy(ufirst).DoDamage(afirst.Price, usecond);
                }
                if (usecond.HitPoints > 0 && usecond is not GorodAdapter)
                {
                    new LogProxy(usecond).DoDamage(asecond.Price, ufirst);
                }
            }
        }
        private void ActivateAbility(int pos, Army my, Army enemy)
        {
            Unit? me = (Unit?)my.List[pos];
            if (me == null || me.SpecialAbilityType == 0)
            {
                return;
            }
            else
            {
                new LogProxy(me).DoAction(pos, my, enemy, strategy);
            }
        }
        public void ExchangeSpecialAbilities()
        {
            int fcount = afirst.List.Count;
            int scount = asecond.List.Count;
            for (int i = 0; i < Math.Max(fcount, scount); i++)
            {
                if (i < fcount)
                {
                    ActivateAbility(i, afirst, asecond);
                }
                if (i < scount)
                {
                    ActivateAbility(i, asecond, afirst);
                }
            }
        }
        public void RemoveDeadUnits()
        {
            strategy.RemoveDeadUnits(afirst.List);
            strategy.RemoveDeadUnits(asecond.List);

            int fcount = afirst.List.Where(x => x == null).ToList().Count();
            int scount = asecond.List.Where(x => x == null).ToList().Count();
            if (afirst.List.Count == fcount || asecond.List.Count == scount)
            {
                Over = true;
            }
        }
        public void NextMove()
        {
            Console.WriteLine("Ход совершен");
            ExchangeAttacks();
            ExchangeSpecialAbilities();
            RemoveDeadUnits();
            state++;
        }


        public class Snapshot
        {
            private Game game;
            private int state;
            private IStrategy strategy;
            private List<object?> af, ase;
            private int afw, asw;
            private bool over;
            public Snapshot(Game game, int state, IStrategy strategy, Army afirst, Army asecond, bool over)
            {
                this.game = game;
                this.state = state;
                this.strategy = strategy;
                
                af = new List<object?>();
                foreach (object o in afirst.List)
                {
                    if (o != null)
                    {
                        af.Add(((Unit?)o).Copy());
                    }
                    else
                    {
                        af.Add(null);
                    }
                }
                ase = new List<object?>();
                foreach (object o in asecond.List)
                {
                    if (o != null)
                    {
                        ase.Add(((Unit?)o).Copy());
                    }
                    else
                    {
                        ase.Add(null);
                    }
                }

                afw = afirst.Wins;
                asw = asecond.Wins;
                this.over = over;
            }
            public void Restore()
            {
                game.state = state;
                game.strategy = strategy;
                game.afirst.List.Clear();
                game.afirst.List.AddRange(af);
                game.afirst.Wins = afw;
                game.asecond.List.Clear();
                game.asecond.List.AddRange(ase);
                game.asecond.Wins = asw;
                game.Over = over;
            }
        }
        public Snapshot CreateSnapshot()
        {
            return new Snapshot(this, state, strategy, afirst, asecond, Over);
        }
    }

    public class LogProxy : IUnit, ISpecialAbility
    {
        private Unit me;
        private string atkpath, specpath, wppath;
        private string msg;
        public int UnitDescriptionId { get; }
        public string UnitName { get; }
        public string Name { get; }
        public Type Type { get; }
        public int Attack { get; }
        public int Defence { get; }
        public int MaxHP { get; }
        public int HitPoints { get; set; }
        public int UnitPrice { get; }
        public int SpecialAbilityType { get; }
        public int SpecialAbilityStrength { get; }
        public int SpecialAbilityRange { get; }
        public LogProxy(Unit unit)
        {
            me = unit;
            this.UnitDescriptionId = me.UnitDescriptionId;
            this.UnitName = me.UnitName;
            this.Name = me.Name;
            this.Type = me.Type;
            this.Attack = me.Attack;
            this.Defence = me.Defence;
            this.MaxHP = me.MaxHP;
            this.HitPoints = me.HitPoints;
            this.UnitPrice = me.UnitPrice;
            this.SpecialAbilityType = me.SpecialAbilityType;
            this.SpecialAbilityStrength = me.SpecialAbilityStrength;
            this.SpecialAbilityRange = me.SpecialAbilityRange;

            atkpath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\attacks.txt"));
            specpath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\specials.txt"));
            wppath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\wipes.txt"));
            msg = "";
        }
        public string DoDamage(int price, Unit enemy)
        {
            using (StreamWriter logger = new StreamWriter(atkpath, true))
            {
                msg = $"Юнит {Name} атакует, ";
                logger.Write(msg);
                logger.Close();
                me.DoDamage(price, enemy);
            }
            return msg;
        }
        public virtual string TakeDamage(int attack, int price)
        {
            using (StreamWriter logger = new StreamWriter(atkpath, true))
            {
                string damage = me.TakeDamage(attack, price);
                msg = $"юнит {Name} теряет " + damage + " здоровья";
                logger.WriteLine(msg);
                logger.Close();
            }
            return msg;
        }
        public void DoAction(int pos, Army one, Army other, IStrategy strategy)
        {
            using (StreamWriter logger = new StreamWriter(specpath, true))
            {
                logger.Write($"{Name} активирует спец. возможность {UnitName}: ");
                logger.Close();
            }
            msg = me.DoAction(pos, one, other, strategy);
            using (StreamWriter logger = new StreamWriter(specpath, true))
            {
                logger.WriteLine(msg);
                logger.Close();
            }
        }
        public void Wipe()
        {
            using (StreamWriter logger = new StreamWriter(wppath, true))
            {
                logger.WriteLine($"Юнит {Name} покидает поле боя");
                me.Wipe();
                logger.Close();
            }
        }
    }

    public class CmdHistory
    {
        public Stack<Game.Snapshot> Past { get; private set; }
        public Stack<Game.Snapshot> Future { get; private set; }
        public CmdHistory(Game game)
        {
            Past = new Stack<Game.Snapshot>();
            Past.Push(game.CreateSnapshot());
            Future = new Stack<Game.Snapshot>();
        }
    }

    public abstract class Command
    {
        protected Game game;
        private CmdHistory history;
        public Command(Game game, CmdHistory history)
        {
            this.game = game;
            this.history = history;
        }
        public void Save()
        {
            history.Past.Push(game.CreateSnapshot());
        }
        public abstract void Execute();
        public virtual void Undo()
        {
            if (history.Past.Count > 1)
            {
                history.Future.Push(history.Past.Pop());
                history.Past.Peek().Restore();
                game.ShowStats();
            }
            else
            {
                Console.WriteLine("Нельзя отменять действия дальше\n");
            }
        }
        public void Redo()
        {
            if (history.Future.Count > 0)
            {
                history.Past.Push(history.Future.Pop());
                history.Past.Peek().Restore();
                game.ShowStats();
            }
            else
            {
                Console.WriteLine("Нельзя восстанавливать действия дальше\n");
            }
        }
    }

    public class MoveCtrl : Command
    {
        public MoveCtrl(Game game, CmdHistory history) : base(game, history) { }
        public override void Execute()
        {
            game.NextMove();
            game.ShowStats();
            Save();
        }
    }

    public interface IStrategy
    {
        int Lines { get; }
        void ShowStats(List<Unit?> one, List<Unit?> two);
        void RemoveDeadUnits(List<object?> army);
        string DoActionArcher(int pos, Army my, Army enemy);
        string DoActionHealer(int pos, Army my, Army enemy);
        string DoActionWitcher(int pos, Army my, Army enemy);
    }

    public class Help
    {
        public void SwitchCase(int i, Unit u)
        {
            switch (i)
            {
                case 0:
                    Console.Write(u.Name);
                    break;
                case 1:
                    Console.Write(u.UnitName);
                    break;
                case 2:
                    Console.Write(u.HitPoints);
                    break;
                case 3:
                    Console.Write(u.Attack);
                    break;
                case 4:
                    Console.Write(u.Defence);
                    break;
                case 5:
                    Console.Write(u.SpecialAbilityStrength);
                    break;
                case 6:
                    Console.Write(u.SpecialAbilityRange);
                    break;
            }
        }
        public void Print(int i, int j, int add, List<Unit?> units)
        {
            int pos = 3 * j + add;
            if (pos > (units.Count - 1) || units[pos] == null)
            {
                Console.Write("\t-");
                return;
            }
            Console.Write("\t");
            Unit? u = units[pos];
            SwitchCase(i, u);
        }
        public object? Next(List<object?> units, int i)
        {
            object? current = units[i];
            object? next;
            if ((i + 3) >= 0 && (i + 3) < units.Count)
            {
                next = Next(units, i + 3);
            }
            else
            {
                next = null;
            }
            units.RemoveAt(i);
            units.Insert(i, next);
            return current;
        }
    }

    public partial class OneOnOne : IStrategy
    {
        public int Lines { get; } = 1;
        public void ShowStats(List<Unit?> one, List<Unit?> two)
        {
            List<Unit?> everyone = one.Reverse<Unit?>().ToList().Concat(two).ToList();
            const int rows = 7;
            int columns = everyone.Count;
            int separator = one.Count;
            List<string> column = new List<string>() { "Имя", "Класс", "ХП", "Атака", "Защита", "Сила", "Радиус" };

            for (int i = 0; i < rows; i++)
            {
                Console.Write(column[i]);
                for (int j = 0; j < columns; j++)
                {
                    if (j == 0 || j == separator)
                    {
                        Console.Write("\t\t");
                    }
                    else
                    {
                        Console.Write("\t");
                    }
                    Unit? u = everyone[j];
                    new Help().SwitchCase(i, u);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public void RemoveDeadUnits(List<object?> army)
        {
            foreach (object? u in army)
            {
                if (((Unit?)u).HitPoints <= 0)
                {
                    new LogProxy((Unit?)u).Wipe();
                }
            }
            army.Where(u => ((Unit?)u).HitPoints <= 0).ToList().ForEach(u => army.Remove(u));
        }

        public string DoActionArcher(int pos, Army my, Army enemy)
        {
            Unit me = (Unit)my.List[pos];
            LogProxy proxyme = new LogProxy(me);

            int barrier = Math.Min(me.SpecialAbilityRange - pos, enemy.List.Count);
            if (barrier < 1)
            {
                return "не хватило дальности";
            }
            for (int i = 0; i < barrier; i++)
            {
                Unit en = (Unit)enemy.List[i];
                if (en.HitPoints > 0)
                {
                    return proxyme.DoDamage(my.Price, en);
                }
            }
            return "все оппоненты в пределах досягаемости оказались мертвы";
        }
        public string DoActionHealer(int pos, Army my, Army enemy)
        {
            Unit me = (Unit)my.List[pos];

            int barrier1 = Math.Max(pos - me.SpecialAbilityRange, 0);
            int barrier2 = Math.Min(pos + me.SpecialAbilityRange, my.List.Count - 1);
            for (int i = barrier1; i <= barrier2; i++)
            {
                Unit? fr = (Unit?)my.List[i];
                if (fr != null)
                {
                    bool check = typeof(IHealable).IsAssignableFrom(fr.Type);
                    if (check && fr.HitPoints > 0 && fr.HitPoints < fr.MaxHP)
                    {
                        IHealable.GainHealth(me.SpecialAbilityStrength, fr);
                        return $"юнит {fr.Name} излечился до {fr.HitPoints} здоровья";
                    }
                }
            }
            return "все дружеские юниты в пределах досягаемости либо мертвы, либо здоровы";
        }
        public string DoActionWitcher(int pos, Army my, Army enemy)
        {
            Unit me = (Unit)my.List[pos];

            int barrier1 = Math.Max(pos - me.SpecialAbilityRange, 0);
            int barrier2 = Math.Min(pos + me.SpecialAbilityRange, my.List.Count - 1);
            for (int i = barrier1; i <= barrier2; i++)
            {
                Unit? fr = (Unit?)my.List[i];
                if (my.List[i] != null)
                {
                    bool check = typeof(ICloneableUnit).IsAssignableFrom(fr.Type);
                    if (check && fr.HitPoints > 0)
                    {
                        var clone = ((LightInfantry)my.List[i]).Clone();
                        if (clone != null)
                        {
                            my.List.Insert(i, clone);
                            return $"юнит {fr.Name} был клонирован с новым именем {((Unit)clone).Name}";
                        }
                    }
                }
            }
            return $"не получилось применить способность";
        }
    }

    public class ThreeOnThree : IStrategy
    {
        public int Lines { get; } = 3;
        public void ShowStats(List<Unit?> one, List<Unit?> two)
        {
            Help h = new Help();
            const int rows = 7;
            int amount1 = (int)Math.Ceiling(one.Count / (decimal)Lines);
            int amount2 = (int)Math.Ceiling(two.Count / (decimal)Lines);
            List<string> column = new List<string>() { "Имя", "Класс", "ХП", "Атака", "Защита", "Сила", "Радиус" };
            string dashes = string.Concat(Enumerable.Repeat("--------", amount1 + amount2 + 3));

            Console.WriteLine(dashes);
            for (int i = 0; i < Lines; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Console.Write(column[j] + "\t");
                    for (int k = amount1 - 1; k >= 0; k--)
                    {
                        h.Print(j, k, i, one);
                    }
                    Console.Write("\t");
                    for (int k = 0; k < amount2; k++)
                    {
                        h.Print(j, k, i, two);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine(dashes);
            }
            Console.WriteLine();
        }
        public void RemoveDeadUnits(List<object?> army)
        {
            for (int i = 0; i < army.Count; i++)
            {
                if (army[i] != null && ((Unit?)army[i]).HitPoints <= 0)
                {
                    object deleted = new Help().Next(army, i);
                    new LogProxy((Unit)deleted).Wipe();
                }
            }
        }

        private bool DoDamage(Unit me, Unit? en, int price, int i, int column, out string result, int rowdiff = 0)
        {
            result = "";
            double range = Math.Sqrt(Math.Pow(i / 3 + column + 1, 2) + rowdiff);
            if (en != null && range <= me.SpecialAbilityRange && en.HitPoints > 0)
            {
                result = new LogProxy(me).DoDamage(price, en);
                return true;
            }
            return false;
        }
        public string DoActionArcher(int pos, Army my, Army enemy)
        {
            Unit me = (Unit)my.List[pos];
            List<Unit?> enemies = enemy.List.Select(x => (Unit?)x).ToList();
            int row = pos % 3;
            int column = pos / 3;
            
            for (int i = row; i < enemies.Count; i += 3)
            {
                if (DoDamage(me, enemies[i], my.Price, i, column, out string result))
                {
                    return result;
                }
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (i % 3 == row)
                {
                    continue;
                }
                if (DoDamage(me, enemies[i], my.Price, i, column, out string result, 1))
                {
                    return result;
                }
            }
            return $"все оппоненты в пределах досягаемости оказались мертвы";
        }

        private bool DoHeal(Unit me, Unit? fr, int i, int column, out string result, int rowdiff = 0)
        {
            result = "";
            double range = Math.Sqrt(Math.Pow(i / 3 - column, 2) + rowdiff);
            if (fr != null)
            {
                bool check = typeof(IHealable).IsAssignableFrom(fr.Type);
                if (check && range <= me.SpecialAbilityRange && fr.HitPoints > 0 && fr.HitPoints < fr.MaxHP)
                {
                    IHealable.GainHealth(me.SpecialAbilityStrength, fr);
                    result = $"юнит {fr.Name} излечился до {fr.HitPoints} здоровья";
                    return true;
                }
            }
            return false;
        }
        public string DoActionHealer(int pos, Army my, Army enemy)
        {
            Unit me = (Unit)my.List[pos];
            List<Unit?> units = my.List.Select(x => (Unit?)x).ToList();
            int row = pos % 3;
            int column = pos / 3;

            for (int i = row; i < units.Count; i += 3)
            {
                if (DoHeal(me, units[i], i, column, out string result))
                {
                    return result;
                }
            }
            for (int i = 0; i < units.Count; i++)
            {
                if (i / 3 == row)
                {
                    continue;
                }
                if (DoHeal(me, units[i], i, column, out string result, 1))
                {
                    return result;
                }
            }
            return $"все дружеские юниты в пределах досягаемости либо мертвы, либо здоровы";
        }

        private bool DoClone(Unit me, List<object?> army, int i, int column, out string result, int rowdiff = 0)
        {
            result = "";
            double range = Math.Sqrt(Math.Pow(i / 3 - column, 2) + rowdiff);
            Unit? fr = (Unit?)army[i];
            if (army[i] != null)
            {
                bool check = typeof(ICloneableUnit).IsAssignableFrom(fr.Type);
                if (check && range <= me.SpecialAbilityRange && fr.HitPoints > 0)
                {
                    var clone = ((LightInfantry)army[i]).Clone();
                    if (clone != null)
                    {
                        army.Insert(i, clone);
                        result = $"юнит {fr.Name} был клонирован с новым именем {((Unit)clone).Name}";
                        return true;
                    }
                }
            }
            return false;
        }
        public string DoActionWitcher(int pos, Army my, Army enemy)
        {
            Unit me = (Unit)my.List[pos];
            int row = pos % 3;
            int column = pos / 3;

            for (int i = row; i < my.List.Count; i += 3)
            {
                if (DoClone(me, my.List, i, column, out string result))
                {
                    return result;
                }
            }
            for (int i = 0; i < my.List.Count; i++)
            {
                if (i / 3 == row)
                {
                    continue;
                }
                if (DoClone(me, my.List, i, column, out string result, 1))
                {
                    return result;
                }
            }
            return $"не получилось применить способность";
        }
    }

    public class WallToWall : IStrategy
    {
        public int Lines { get; private set; }
        public void ShowStats(List<Unit?> one, List<Unit?> two)
        {
            Lines = Math.Max(one.Count, two.Count);
            const int rows = 7;
            List<string> column = new List<string>() { "Имя", "Класс", "ХП", "Атака", "Защита", "Сила", "Радиус" };

            Console.WriteLine("---------------------------------------------");
            for (int i = 0; i < Lines; i++)
            {
                Unit u;
                List<string> uone = Enumerable.Repeat("-", rows).ToList();
                List<string> utwo = new List<string>();
                utwo.AddRange(uone);

                if (i < one.Count && one[i] != null)
                {
                    u = one[i];
                    uone = new List<string>() { u.Name, u.UnitName, u.HitPoints.ToString(), u.Attack.ToString(),
                    u.Defence.ToString(), u.SpecialAbilityStrength.ToString(), u.SpecialAbilityRange.ToString() };
                }
                if (i < two.Count && two[i] != null)
                {
                    u = two[i];
                    utwo = new List<string>() { u.Name, u.UnitName, u.HitPoints.ToString(), u.Attack.ToString(),
                    u.Defence.ToString(), u.SpecialAbilityStrength.ToString(), u.SpecialAbilityRange.ToString() };
                }

                for (int j = 0; j < rows; j++)
                {
                    Console.WriteLine($"{column[j]}\t\t{uone[j]}\t\t{utwo[j]}");
                }
                Console.WriteLine("---------------------------------------------");
            }
            Console.WriteLine();
        }
        public void RemoveDeadUnits(List<object?> army)
        {
            for (int i = 0; i < army.Count; i++)
            {
                if (army[i] != null && ((Unit?)army[i]).HitPoints <= 0)
                {
                    new LogProxy((Unit)army[i]).Wipe();
                    army.RemoveAt(i);
                    army.Insert(i, null);
                }
            }
        }

        private bool DoDamage(Unit me, Unit? en, int price, out string result)
        {
            result = "";
            if (en != null && en.HitPoints > 0)
            {
                result = new LogProxy(me).DoDamage(price, en);
                return true;
            }
            return false;
        }
        public string DoActionArcher(int pos, Army my, Army enemy)
        {
            Unit me = (Unit)my.List[pos];
            List<Unit?> enemies = enemy.List.Select(x => (Unit?)x).ToList();
            int rowdiff = (int)Math.Floor(Math.Sqrt(Math.Pow(me.SpecialAbilityRange, 2) - 1));

            if (pos < enemies.Count)
            {
                Unit? en = enemies[pos];
                if (DoDamage(me, en, my.Price, out string result))
                {
                    return result;
                }

                int botline = Math.Max(0, pos - rowdiff);
                int topline = Math.Min(enemies.Count, pos + rowdiff);
                for (int i = botline; i < topline; i++)
                {
                    if (i == pos)
                    {
                        continue;
                    }
                    en = enemies[i];
                    if (DoDamage(me, en, my.Price, out string res))
                    {
                        return res;
                    }
                }
            }
            return $"все оппоненты в пределах досягаемости оказались мертвы";
        }
        public string DoActionHealer(int pos, Army my, Army enemy)
        {
            return new OneOnOne().DoActionHealer(pos, my, enemy);
        }
        public string DoActionWitcher(int pos, Army my, Army enemy)
        {
            return new OneOnOne().DoActionWitcher(pos, my, enemy);
        }
    }

    public class StrategyCtrl : Command
    {
        public StrategyCtrl(Game game, CmdHistory history) : base(game, history) { }

        public override void Execute()
        {
            Console.WriteLine("\n0. Вернуться в меню\n" +
                "1. Один на один\n" +
                "2. Трое на трое\n" +
                "3. Стенка на стенку\n");
            bool correct = int.TryParse(Console.ReadLine(), out int input);
            if (!correct) input = 404;
            switch (input)
            {
                case 0:
                    return;
                case 1:
                    game.SetStrategy(new OneOnOne());
                    break;
                case 2:
                    game.SetStrategy(new ThreeOnThree());
                    break;
                case 3:
                    game.SetStrategy(new WallToWall());
                    break;
                default:
                    break;
            }
            game.RegroupUnits();
            game.ShowStats();
            Save();
        }
    }

    public class CmdInvoker
    {
        Command command;
        public void SetCommand(Command command)
        {
            this.command = command;
        }
        public void Execute()
        {
            command.Execute();
        }
        public void Undo()
        {
            command.Undo();
        }
        public void Redo()
        {
            command.Redo();
        }
    }

    public class WarGame // client
    {
        const int fights = 4;
        bool steps;
        Game game; // receiver 1
        CmdHistory? history; // receiver 2
        CmdInvoker invoker = new CmdInvoker(); // invoker
        MoveCtrl mctrl; // concrete command 1
        StrategyCtrl sctrl; // concrete command 2
        public Army War(Army user, Army computer)
        {
            Army[] armies = new Army[] { user, computer };
            Console.WriteLine("Начало войны\n");
            for (int i = 0; i < fights; i++)
            {
                Fight(armies[i % 2], armies[(i + 1) % 2]);
            }
            return FindTotalWinner(user, computer);
        }
        private void Fight(Army afirst, Army asecond) {
            Start(afirst, asecond);
            Console.WriteLine($"НАЧАЛО БОЯ\n");
            game.ShowStats();

            int input, amount;
            amount = 0;
            while (true) {
                if (game.Over || amount >= 10)
                {
                    FindLocalWinner(afirst, asecond);
                    End(afirst, asecond);
                    return;
                }
                invoker.SetCommand(mctrl);
                if (steps)
                {
                    Console.WriteLine("0. Закончить игру\n" +
                        "1. Проиграть бой до конца\n" +
                        "2. Сделать ход\n" +
                        "3. Сменить стратегию\n" +
                        "4. Отменить последнее действие\n" +
                        "5. Повторить отмененное действие\n");
                    bool correct = int.TryParse(Console.ReadLine(), out input);
                    if (!correct) input = 404;
                }
                else
                {
                    input = 2;
                }
                switch (input)
                {
                    case 0:
                        Environment.Exit(0);
                        break;
                    case 1 or 2:
                        if (input == 1)
                        {
                            steps = false;
                        }
                        invoker.Execute();
                        amount++;
                        break;
                    case 3:
                        invoker.SetCommand(sctrl);
                        invoker.Execute();
                        break;
                    case 4:
                        invoker.Undo();
                        break;
                    case 5:
                        invoker.Redo();
                        break;
                    default:
                        break;
                }
            }
        }
        private void Start(Army afirst, Army asecond)
        {
            steps = true;
            int state = 0;
            if (history != null)
            {
                state = history.Past.Count;
            }
            invoker = new CmdInvoker();
            game = new Game(afirst, asecond, state);
            history = new CmdHistory(game);
            mctrl = new MoveCtrl(game, history);
            sctrl = new StrategyCtrl(game, history);
        }
        private void End(Army one, Army other)
        {
            one.List.Clear();
            other.List.Clear();
            one.List.AddRange(one.InitialList);
            other.List.AddRange(other.InitialList);
            foreach (Unit u in one.List.Concat(other.List))
            {
                u.HitPoints = u.MaxHP;
            }
        }
        private void FindLocalWinner(Army one, Army other)
        {
            if (one.List.Count > other.List.Count)
            {
                Console.WriteLine($"В этом бою победила армия {one.Name}\n\n\n");
                one.Wins++;
            }
            else if (one.List.Count < other.List.Count)
            {
                Console.WriteLine($"В этом бою победила армия {other.Name}\n\n\n");
                other.Wins++;
            }
            else
            {
                Console.WriteLine("В этом бою ничья\n\n\n");
            }
        }
        private Army FindTotalWinner(Army one, Army other)
        {
            if (one.Wins > other.Wins)
            {
                Console.WriteLine($"Победила армия {one.Name}");
                return one;
            }
            else if (one.Wins < other.Wins)
            {
                Console.WriteLine($"Победила армия {other.Name}");
                return other;
            }
            else
            {
                Console.WriteLine("Ничья");
                return new Army[] { one, other }[new Random().Next(0, 1)];
            }
        }
    }

    class GetJsonFields
    {
        public string? TeamName { get; set; }
        public List<Unit>? UnitDiscriptions { get; set; }
        public List<int>? Units { get; set; }
        public void createJsonFile(GetJsonFields jsonString)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };
            string json = JsonSerializer.Serialize(jsonString, options);
            File.WriteAllText("WhyNot.json", json);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Army user = Army.CreateUserArmy();
            Army computer = Army.CreateRandomArmy();
            Army winner = new WarGame().War(user, computer);

            List<Unit> army = winner.InitialList.Select(x => (Unit)x).ToList();

            List<int> id = new();
            for (int i = 0; i < army.Count; i++)
            {
                id.Add(army[i].UnitDescriptionId);
            }

            var jsonFile = new GetJsonFields
            {
                TeamName = "WhyNot?",
                UnitDiscriptions = army,
                Units = id
            };
            jsonFile.createJsonFile(jsonFile);
        }
    }
}