using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class Work
    {
        public int Id { get; set; }
        public int EstimateId { get; set; }
        public Estimate Estimate { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
    }
}
