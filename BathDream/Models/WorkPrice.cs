using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class WorkPrice
    {
        public static implicit operator Work(WorkPrice wp) => new()
        {
            InnerName = wp.InnerName,
            Name = wp.Name,
            Unit = wp.Unit,
            Price = wp.Price
        };

        public int Id { get; set; }
        public string InnerName { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
    }
}
