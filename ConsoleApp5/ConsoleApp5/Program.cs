using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ComputerClub computerClub = new ComputerClub(10);
            computerClub.Work();
        }

    }

    class ComputerClub
    {
        private int _money;
        private List<Computer> _computers = new List<Computer>();
        private Queue<Client> _clients = new Queue<Client>();
        public ComputerClub(int computersCount)
        {
            Random random = new Random();
            for (int i = 0; i < computersCount; i++)
            {
                _computers.Add(new Computer(random.Next(5, 15)));
            }
            CreateNewClients(25, random);
        }

        public void CreateNewClients(int count, Random random)
        {
            for (int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(random.Next(100, 250), random));
            }
                
            
        }

        public void Work()
        {
            
            while (_clients.Count > 0)
            {
                bool check = true;
                Client newClient = _clients.Dequeue();
                Console.WriteLine($"Баланс компьютерного клуба {_money} руб. Ждем нового клиента");
                Console.WriteLine($"У вас новый клиент и он хочет купить {newClient.DesiredMinutes}  минут");
                ShowAllComputerState();
                Console.Write("\nВы предлагаете ему компьютер: ");
                string userInput = Console.ReadLine();
                while (check)
                {
                    if(int.TryParse(userInput, out int computerNumber))
                    {
                        computerNumber -= 1;
                        if (computerNumber>=0 && computerNumber<_computers.Count)
                        {
                            if(_computers[computerNumber].IsTaken)
                            {
                                Console.WriteLine("Компьютер занят.");
                                Console.Write("\nВы предлагаете ему компьютер: ");
                                userInput = Console.ReadLine();
                            }
                            else
                            {
                                if (newClient.CheckSolvency(_computers[computerNumber]))
                                {
                                    Console.WriteLine("Клиент заплатил и сел за компьютер номер " + (computerNumber + 1));
                                    _money += newClient.Pay();
                                    _computers[computerNumber].BecomeTaken(newClient);
                                    check = false;
                                }
                                else
                                {
                                    Console.WriteLine("У клиента не хватило денег и он ушел.");
                                    check = false;
                                    CreateNewClients(1, new Random());
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Неверный номер компьютера. Введите номер от 1 до {_computers.Count}.");
                            Console.Write("\nВы предлагаете ему компьютер:");
                            userInput = Console.ReadLine();
                        }
                    }
                    else
                    {
                        
                        Console.WriteLine("Неверный ввод! Повторите ввод снова");
                        Console.Write("\nВы предлагаете ему компьютер:");
                        userInput = Console.ReadLine();
                    }

                }

                Console.WriteLine($"Осталось клиентов {_clients.Count}");
                Console.WriteLine("Чтобы принять следующего клиента нажмите любую кнопку.");
                Console.ReadKey();
                Console.Clear();
                SpendOneMinute();
            }
        }

        private void ShowAllComputerState()
        {
            Console.WriteLine("\nСписок всех компьютеров:");
            for(int i = 0; i < _computers.Count; i++)
            {
                Console.Write(i + 1 + "-");
                _computers[i].ShowState();

            }
        }

        private void SpendOneMinute()
        {
            foreach(var computer in _computers)
            {
                computer.SpendOneMinute();
            }
        }
    }

    class Computer
    {
        private Client _client;
        private int _minutesRemaining;
        public bool IsTaken
        {
            get
            {
                return _minutesRemaining > 0;
            }
        }
        public int PricePerMinute { get; private set; }

        public Computer(int pricePerMinute)
        {
            PricePerMinute = pricePerMinute;
        }

        public void BecomeTaken(Client client)
        {
            _client = client;
            _minutesRemaining = _client.DesiredMinutes;
        }

        public void BecomeEmpty()
        {
            _client = null;
        }

        public void SpendOneMinute()
        {
            _minutesRemaining--;
        }

        public void ShowState()
        {
            if (IsTaken)
                Console.WriteLine($"Компьютер занят, осталось минут:{_minutesRemaining}");
            else
                Console.WriteLine($"Компьютер свободен, цена за минуту:{PricePerMinute}");
        }
    }

    class Client
    {
        private int _money;
        private int _moneyToPay;

        public int DesiredMinutes { get; private set; }

        public Client(int money, Random random)
        {
            _money = money;
            DesiredMinutes = random.Next(10, 30);
        }

        public bool CheckSolvency(Computer computer)
        {
            _moneyToPay= DesiredMinutes * computer.PricePerMinute;
            if (_money >= _moneyToPay)
            {
                return true;
            }
            else
            {
                _moneyToPay = 0;
                return false;
            }
        }

        public int Pay()
        {
            _money-= _moneyToPay;
            return _moneyToPay;
        }
    }


}