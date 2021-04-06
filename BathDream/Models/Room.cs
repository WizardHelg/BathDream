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

        public double FloorArea() => Math.Round(Width / 1000 * Length / 1000, 2, MidpointRounding.AwayFromZero);
        public double CeilingArea() => Math.Round(Width / 1000 * Length / 1000, 2, MidpointRounding.AwayFromZero);
        public double DoorArea() => Math.Round(DoorWidth / 1000 * DoorHeight / 1000, 2, MidpointRounding.AwayFromZero);
        public double WallsArea() => Math.Round(2 * (Width / 1000 + Length / 1000) * Height / 1000 - DoorWidth / 1000 * DoorHeight / 1000, 2, MidpointRounding.AwayFromZero);
    }
}
