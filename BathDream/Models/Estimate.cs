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

        /// <summary>
        /// Сумма всей сметы
        /// </summary>
        /// <returns></returns>
        public double Total()
        {
            return Works.Sum(w => w.Total);
        }
        /// <summary>
        /// Уникальные работы
        /// </summary>
        /// <returns></returns>
        public List<Work> UniqueWorks()
        {
            List<Work> UniqueWorks = new List<Work>();
            foreach (var work in Works)
            {
                if (UniqueWorks.Any(w => w.WorkType.Id == work.WorkType.Id))
                {
                    continue;
                }
                UniqueWorks.Add(work);
            }
            return UniqueWorks;
        }
    }
}
