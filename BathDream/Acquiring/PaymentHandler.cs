using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Acquiring
{
    public class PaymentHandler
    {
        public AlfabankPaymentAPI AlfabankPayment { get; set; }
        public int StatusCode { get; set; }

        private readonly Dictionary<string, string> Status = new Dictionary<string, string>()
        {
            ["0"] = "Заказ зарегистрирован, но не оплачен",
            ["1"] = "Предавторизованная сумма захолдирована (для двухстадийных платежей)",
            ["2"] = "Проведена полная авторизация суммы заказа",
            ["3"] = "Авторизация отменена",
            ["4"] = "По транзакции была проведена операция возврата",
            ["5"] = "Инициирована авторизация через ACS банка-эмитента",
            ["6"] = "Авторизация отклонена"
        };

        public PaymentHandler()
        {
            AlfabankPayment = new AlfabankPaymentAPI("bath_dream-api", "bath_dream*?1");
        }

        /// <summary>
        /// Создание заказа для оплаты
        /// </summary>
        /// <param name="paymentNumber">Номер заказа в системе магазина</param>
        /// <param name="amount">Сумма платежа в копейках</param>
        /// <param name="returnUrl">Адрес для перенаправления в случае успешной оплаты</param>
        public void CreatePayment(string paymentNumber, int amount, string returnUrl, string failUrl)
        {
            AlfabankPayment.Registred(
                new Dictionary<string, string>
                {
                    ["orderNumber"] = paymentNumber,
                    ["amount"] = amount.ToString(),
                    ["returnUrl"] = returnUrl,
                    ["failUrl"] = failUrl
                });
        }

        /// <summary>
        /// Текущее состояние заказа
        /// </summary>
        /// <param name="paymentId">Номер заказа в платежной системе</param>
        /// <returns>Возвращает текущее состояние заказа</returns>
        public string GetPaymentStatus(string paymentId)
        {
            AlfabankPayment.GetOrderStatus(
                new Dictionary<string, string>
                {
                    ["orderId"] = paymentId
                });
            return Status.FirstOrDefault(s => s.Key == AlfabankPayment.OrderStatus).Key;
        }

        /// <summary>
        /// Проверяет, оплачен заказ или нет
        /// </summary>
        /// <param name="paymentId">Номер заказа в платежной системе</param>
        /// <returns></returns>
        public bool OnPaid(string paymentId, out string status)
        {
            status = GetPaymentStatus(paymentId);
            if (AlfabankPayment?.OrderStatus == "2")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }   
}
