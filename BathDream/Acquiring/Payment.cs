using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Acquiring
{
    public class Payment
    {
        public void CreatePayment()
        {
            AlfabankPaymentAPI alfabankPayment = new AlfabankPaymentAPI("TESTUSER", "TESTPASSWORD");
            alfabankPayment.Registred(
                new Dictionary<string, string>
                {
                    ["userName"] = "NAME",
                    ["password"] = "PASSWORD",
                    ["orderNumber"] = "ORDERNUMBER",
                    ["amount"] = "AMOUNT",
                    ["returnUrl"] = "RETURNURL",
                    ["pageView"] = "PAGEVIEW"
                });


            //TinkoffPaymentAPI tinkoffPayment = new TinkoffPaymentAPI("1598805344397DEMO", "3pckn4c1uk7xc3x0");

            //tinkoffPayment.Init(new Dictionary<string, string> 
            //{
            //    ["Amount"] = "2000",
            //    ["OrderId"] = "Test1-1-1"
            //});
            //string res = tinkoffPayment.Response;
        }
    }
}
