using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Models
{
    public class Payment
    {
        public int Id { get; set; }

        /// <summary>
        /// Номер заказа в платежной системе
        /// </summary>
        public string PaymentId { get; set; }

        /// <summary>
        /// Состояние заказа в платежной системе
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Номер заказа в системе магазина
        /// </summary>
        public string PaymentNumber { get; set; }

        /// <summary>
        /// Дата и время заказа
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Сумма платежа в копейках
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Заказ (Order)
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// Заказ (Order)
        /// </summary>
        public OrderMaterial OrderMaterial { get; set; }

        /// <summary>
        /// Описание заказа
        /// </summary>
        public string Description { get; set; }
    }
}
