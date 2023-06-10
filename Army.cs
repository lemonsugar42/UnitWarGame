using System.ComponentModel;

namespace gamedraft
{
    public class Army
    {
        public string Name { get; private set; }
        public int Wins { get; set; }
        public int Price { get; private set; }
        public List<object?> List { get; set; }
        public List<object?> InitialList { get; private set; }
        Army(string name, List<int> amounts)
        {
            Name = name;
            Wins = 0;
            List = new List<object?>() { };
            List<UnitFactory> factories = new List<UnitFactory>() { new SimpleFactory(), new ArcherFactory(), new HealerFactory(), new WitcherFactory() };
            for (int n = 0; n < factories.Count; n++)
            {
                UnitFactory factory = factories[n];
                for (int i = 0; i < amounts[4 * n]; i++)
                {
                    List.Add(factory.CreateLight());
                }
                for (int i = 0; i < amounts[4 * n + 1]; i++)
                {
                    List.Add(factory.CreateHeavy());
                }
                for (int i = 0; i < amounts[4 * n + 2]; i++)
                {
                    List.Add(factory.CreateKnight());
                }
                for (int i = 0; i < amounts[4 * n + 3]; i++)
                {
                    List.Add(factory.CreateGorod(new GulyayGorod(24, 24, 48)));
                }
            }

            InitialList = new List<object?>();
            InitialList.AddRange(List);

            foreach (object? u in List)
            {
                if (u != null)
                {
                    Price += ((Unit)u).UnitPrice;
                }
            }
        }

        public static Army CreateUserArmy()
        {
            Console.Write("ВАШ КАПИТАЛ? ");
            int money = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            List<int> amounts = Enumerable.Repeat(0, 16).ToList();
            List<int> prices = new List<int>() { 13, 21, 42, 54, 25, 33, 54, 66, 25, 33, 54, 66, 25, 33, 54, 66 };

            while (true)
            {
                if (money < 13)
                {
                    break;
                }
                Console.WriteLine($"ОСТАЛОСЬ ДЕНЕГ {money}\n" +
                    "Выберите юнит:\n" +
                    "---\t\t1.\t2.\t3.\t4.\n" +
                    "Класс\t\tLightI\tHeavyI\tKnight\tGorod\n" +
                    "ХП\t\t7\t7\t24\t30\n" +
                    "Атака\t\t3\t7\t10\t0\n" +
                    "Защита\t\t3\t7\t8\t24\n" +
                    "ЦЕНА ОТ\t\t13\t21\t42\t54\n");
                bool correct = int.TryParse(Console.ReadLine(), out int input1);
                if (!correct || input1 < 0 || input1 > 4)
                {
                    Console.WriteLine("Некорректный ввод, попробуйте еще\n");
                    continue;
                }
                else if (input1 == 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("\nВыберите разновидность юнита:\n" +
                        "---\t\t1.\t2.\t3.\t4.\n" +
                        "Абилка\t\t---\tArcher\tHealer\tWizard\n" +
                        "Сила\t\t0\t3\t4\t3\n" +
                        "Радиус\t\t0\t3\t2\t3\n" +
                        "ЦЕНА +\t\t0\t12\t12\t12\n");
                    correct = int.TryParse(Console.ReadLine(), out int input2);
                    if (!correct || input2 < 1 || input2 > 4)
                    {
                        Console.WriteLine("Некорректный ввод, попробуйте еще\n");
                        continue;
                    }
                    else
                    {
                        int i = (input2 - 1) * 4 + (input1 - 1);
                        if (money - prices[i] < 0)
                        {
                            Console.WriteLine("Не удалось добавить: деньги закончились\n");
                            continue;
                        }
                        amounts[i]++;
                        money -= prices[i];
                        Console.WriteLine("\nДобавлено успешно\n");
                    }
                }
            }
            return new Army("Пользователь", amounts);
        }

        public static Army CreateRandomArmy()
        {
            List<int>[] options =
            {
                new List<int> { 1, 1, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new List<int> { 0, 2, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new List<int> { 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new List<int> { 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new List<int> { 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },

                new List<int> { 1, 1, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0 },
                new List<int> { 0, 2, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 },
                new List<int> { 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
                new List<int> { 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 },
                new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0 },

                new List<int> { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0},
                new List<int> { 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 },
                new List<int> { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0 },
                new List<int> { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 },
                new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0 },
            };
            int n = new Random().Next(0, 14);
            List<int> amounts = options[n];

            return new Army("Компьютер", amounts);
        }
    }

}