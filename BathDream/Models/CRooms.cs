using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;
using System.Globalization;

namespace BathDream.Models
{
    public class CRooms : ISumAreas
    {
        public double SumFloorArea { get; set; }
        public double SumCeilingArea { get; set; }
        public double SumWallsArea { get; set; }

        [BindProperty(Name = "room_name")]
        public string[] Names { get; set; }

        [BindProperty(Name = "room_length")]
        public string[] Lengths { get; set; }

        [BindProperty(Name = "room_width")]
        public string[] Widths { get; set; }

        [BindProperty(Name = "room_height")]
        public string[] Heights { get; set; }

        [BindProperty(Name = "door_width")]
        public string[] DoorWidths { get; set; }

        [BindProperty(Name = "door_height")]
        public string[] DoorHeights { get; set; }

        public int Count() => Names.GetLength(0);

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Names.GetLength(0); i++)
                yield return new CRoom
                {
                    Name = Names[i],
                    Width = double.TryParse(Widths[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double resW) ? resW : 0.0,
                    Height = double.TryParse(Heights[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double resH) ? resH : 0.0,
                    Length = double.TryParse(Lengths[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double resL) ? resL : 0.0,
                    Door = new Door()
                    {
                        Width = double.TryParse(DoorWidths[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double resDW) ? resDW : 0.0,
                        Height = double.TryParse(DoorHeights[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double resDH) ? resDH : 0.0
                    }
                };
        }
    }
}
