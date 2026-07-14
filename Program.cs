using System.Globalization;

namespace CarDealership
{
    internal class Program
    {
        private const string FileName = "cars.txt";
        private static readonly List<Car> Cars = new();

        public static void Main()
        {
            LoadCarsFromFile();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=====================================");
                Console.WriteLine("     УПРАВЛЕНИЕ НА АВТОКЪЩА");
                Console.WriteLine("=====================================");
                Console.WriteLine("1. Добавяне на нов автомобил");
                Console.WriteLine("2. Продажба на автомобил");
                Console.WriteLine("3. Проверка на наличност");
                Console.WriteLine("4. Справка за всички автомобили");
                Console.WriteLine("0. Изход");
                Console.WriteLine("=====================================");
                Console.Write("Изберете опция: ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        AddCar();
                        break;
                    case "2":
                        SellCar();
                        break;
                    case "3":
                        CheckAvailability();
                        break;
                    case "4":
                        ShowAllCars();
                        break;
                    case "0":
                        SaveCarsToFile();
                        Console.WriteLine("Данните са записани. Довиждане!");
                        return;
                    default:
                        Console.WriteLine("Невалиден избор. Опитайте отново.");
                        break;
                }

                Pause();
            }
        }

        private static void AddCar()
        {
            Console.WriteLine("--- Добавяне на нов автомобил ---");

            string carId;

            while (true)
            {
                carId = ReadRequiredText("Въведете уникален ID: ");

                if (Cars.Any(c => c.CarId.Equals(carId, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("Вече има автомобил с такъв ID. Въведете друг ID.");
                }
                else
                {
                    break;
                }
            }

            string make = ReadRequiredText("Въведете марка: ");
            string model = ReadRequiredText("Въведете модел: ");
            int year = ReadInt("Въведете година на производство: ", 1886, DateTime.Now.Year + 1);
            decimal price = ReadDecimal("Въведете цена: ", 0);

            Car newCar = new Car()
            {
                CarId = carId,
                Make = make,
                Model = model,
                Year = year,
                Price = price,
                Available = true
            };

            Cars.Add(newCar);
            SaveCarsToFile();

            Console.WriteLine("Автомобилът е добавен успешно и е записан във файла.");
        }

        private static void SellCar()
        {
            Console.WriteLine("--- Продажба на автомобил ---");

            string id = ReadRequiredText("Въведете ID на автомобила за продажба: ");

            Car? car = Cars.FirstOrDefault(c => c.CarId.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (car == null)
            {
                Console.WriteLine("Не е намерен автомобил с този ID.");
                return;
            }

            if (!car.Available)
            {
                Console.WriteLine("Този автомобил вече е продаден.");
                return;
            }

            car.Available = false;
            SaveCarsToFile();

            Console.WriteLine("Автомобилът е маркиран като продаден и файлът е актуализиран.");
        }

        private static void CheckAvailability()
        {
            Console.WriteLine("--- Проверка на наличност ---");
            Console.WriteLine("Търсене по марка и/или модел. Може да оставите едното поле празно.");

            Console.Write("Марка: ");
            string make = Console.ReadLine()?.Trim() ?? string.Empty;

            Console.Write("Модел: ");
            string model = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(make) && string.IsNullOrWhiteSpace(model))
            {
                Console.WriteLine("Трябва да въведете поне марка или модел.");
                return;
            }

            var foundCars = Cars.Where(c =>
                (string.IsNullOrWhiteSpace(make) || c.Make.Contains(make, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(model) || c.Model.Contains(model, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            if (foundCars.Count == 0)
            {
                Console.WriteLine("Няма намерени автомобили по зададените критерии.");
                return;
            }

            foreach (Car car in foundCars)
            {
                Console.WriteLine(car);
            }
        }

        private static void ShowAllCars()
        {
            Console.WriteLine("--- Справка за всички автомобили ---");

            if (Cars.Count == 0)
            {
                Console.WriteLine("Няма въведени автомобили.");
                return;
            }

            foreach (Car car in Cars)
            {
                Console.WriteLine(car);
            }
        }

        private static void LoadCarsFromFile()
        {
            Cars.Clear();

            if (!File.Exists(FileName))
            {
                File.WriteAllText(FileName, string.Empty);
                return;
            }

            string[] lines = File.ReadAllLines(FileName);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split('|');

                if (parts.Length != 6)
                {
                    continue;
                }

                bool validYear = int.TryParse(parts[3], out int year);
                bool validPrice = decimal.TryParse(
                    parts[4],
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal price
                );
                bool validAvailable = bool.TryParse(parts[5], out bool available);

                if (!validYear || !validPrice || !validAvailable)
                {
                    continue;
                }

                Cars.Add(new Car
                {
                    CarId = parts[0],
                    Make = parts[1],
                    Model = parts[2],
                    Year = year,
                    Price = price,
                    Available = available
                });
            }
        }

        private static void SaveCarsToFile()
        {
            List<string> lines = new();

            foreach (Car car in Cars)
            {
                string line = string.Join('|',
                    car.CarId,
                    car.Make,
                    car.Model,
                    car.Year.ToString(CultureInfo.InvariantCulture),
                    car.Price.ToString(CultureInfo.InvariantCulture),
                    car.Available.ToString()
                );

                lines.Add(line);
            }

            File.WriteAllLines(FileName, lines);
        }

        private static string ReadRequiredText(string message)
        {
            while (true)
            {
                Console.Write(message);

                string value = Console.ReadLine()?.Trim() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }

                Console.WriteLine("Полето не може да бъде празно.");
            }
        }

        private static int ReadInt(string message, int min, int max)
        {
            while (true)
            {
                Console.Write(message);

                string input = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(input, out int number) && number >= min && number <= max)
                {
                    return number;
                }

                Console.WriteLine($"Въведете валидно цяло число между {min} и {max}.");
            }
        }

        private static decimal ReadDecimal(string message, decimal min)
        {
            while (true)
            {
                Console.Write(message);

                string input = (Console.ReadLine() ?? string.Empty).Replace(',', '.');

                if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number) && number >= min)
                {
                    return number;
                }

                Console.WriteLine($"Въведете валидна цена, по-голяма или равна на {min}.");
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Натиснете Enter за продължение...");
            Console.ReadLine();
        }
    }
}
