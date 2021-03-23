using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public struct CRoom
    {
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public Door Door { get; set; }

        public double FloorArea() => Width / 1000 * Length / 1000;
        public double CeilingArea() => Width / 1000 * Length / 1000;
        public double WallsArea() => 2 * (Width / 1000 + Length / 1000) * Height / 1000 - Door.Area();
    }
}
