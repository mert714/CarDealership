using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership
{
    internal class Car
    {
        public string CarId { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }

        public override string ToString()
        {
            string status = Available ? "Наличен" : "Продаден";
            return $"ID: {CarId} | Марка: {Make} | Модел: {Model} | Година: {Year} | Цена: {Price:F2} € | Статус: {status}";
        }
    }
}
