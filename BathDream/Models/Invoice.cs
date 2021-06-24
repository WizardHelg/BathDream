using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        /// <summary>
        /// Тип заказа: 1 - Смета, 2 - Материалы, 3 - Дополнительные работы
        /// </summary>
        public int Type { get; set; }
        public Order Order { get; set; }
        public string StatusPayment { get; set; }
        public DateTime DateTime { get; set; }
        public List<Material> Materials { get; set; }
        public List<AdditionalWork> AdditionalWorks  { get; set; }
    }
}
