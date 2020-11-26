using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    interface ISumAreas
    {
        public double SumFloorArea { get; set; }
        public double SumCeilingArea { get; set; }
        public double SumWallsArea { get; set; }
    }
}
