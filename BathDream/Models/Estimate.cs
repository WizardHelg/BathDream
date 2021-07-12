using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class Estimate
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Work> Works { get; set; }

        public double Total()
        {
            return Works.Sum(w => w.Total);
        }
    }
}
