using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class AdditionalWork
    {
        public int Id { get; set; }
        public WorkType WorkType { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public Invoice Invoice { get; set; }
    }
}
