using BathDream.Acquiring;
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

        public static List<Payment> CheckStatus(List<Payment> payments)
        {
            PaymentHandler paymentHandler = new PaymentHandler();
            foreach (var payment in payments)
            {
                if ((payment.PaymentStatus == "2" || payment.PaymentStatus == "6") && payment.PaymentStatus == payment.Invoice.StatusPayment)
                {
                    continue;
                }
                string status = paymentHandler.GetPaymentStatus(payment.PaymentId);
                if (payment.PaymentStatus != status
                 || payment.Invoice.StatusPayment != status
                 || payment.PaymentStatus == null)
                {
                    payment.PaymentStatus = status;
                    payment.Invoice.StatusPayment = status;
                }
            }
            return payments;
        }
    }
}
