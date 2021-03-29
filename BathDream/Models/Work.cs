using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class Work
    {
        public int Id { get; set; }
        public string InnerName { get; set; }
        public int EstimateId { get; set; }
        public Estimate Estimate { get; set; }
        public int Position { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public double Volume { get; set; }

        [NotMapped]
        public double Total => Price * Volume;

        [NotMapped]
        public string FirstWord => Name.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[0];

        [NotMapped]
        public string WithoutFirstWord
        {
            get
            {
                string[] buffer = Name.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (buffer.GetLength(0) > 1)
                    return buffer[1];
                else
                    return Name;
            }
        }
    }
}
