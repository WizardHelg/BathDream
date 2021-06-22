using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class OrderMaterial
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public string StatusPayment { get; set; }
        public DateTime DateTime { get; set; }
        public List<Material> Materials { get; set; }
    }
}
