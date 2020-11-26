using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public struct Door
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public double Area() => Width / 1000 * Height / 1000;
    }
}
