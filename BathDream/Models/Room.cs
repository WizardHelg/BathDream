using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int EstimateId { get; set; }
        public Estimate Estimate { get; set; }
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public double DoorWidth { get; set; }
        public double DoorHeight { get; set; }

        public double FloorArea() => Width / 1000 * Length / 1000;
        public double CeilingArea() => Width / 1000 * Length / 1000;
        public double DoorArea() => DoorWidth / 1000 * DoorHeight / 1000;
        public double WallsArea() => 2 * (Width / 1000 + Length / 1000) * Height / 1000 - DoorArea();
    }
}
