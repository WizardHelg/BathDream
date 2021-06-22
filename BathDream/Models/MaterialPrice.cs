using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class MaterialPrice
    {
        public static implicit operator Material(MaterialPrice mp) => new()
        {
            Name = mp.Name,
            Unit = mp.Unit,
            Price = mp.Price
        };
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
    }
}
