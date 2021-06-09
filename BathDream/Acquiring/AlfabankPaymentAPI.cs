using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace BathDream.Acquiring
{
    public class AlfabankPaymentAPI
    {
        private string Username { get; set; }
        private string Password { get; set; }
        private string GatewayUrl { get; set; }
        public string ReturnUrl { get; private set; }

        public string Response { get; private set; }
        public string ErrorMessage { get; private set; }

        public string PaymentId { get; private set; }
        public string PaymentUrl { get; private set; }
        public string OrderStatus { get; private set; }

        public AlfabankPaymentAPI(string username, string password)
        {
            GatewayUrl = "https://web.rbsuat.com/ab/";
            Username = username;
            Password = password;
        }

        private string Gateway(string method, Dictionary<string, string> data)
        {
            string response;

            string url = CombineUrl(GatewayUrl, method);

            using (WebClient webClient = new WebClient())
            {
                NameValueCollection nameValueCollection = new NameValueCollection();
                foreach (var item in data)
                {
                    nameValueCollection.Add(item.Key, item.Value);
                }
                var byteResponse = webClient.UploadValues(url, nameValueCollection);
                response = System.Text.Encoding.UTF8.GetString(byteResponse);
            }

            return response;
        }

        public void Registred(Dictionary<string,string> data)
        {
            string response = Gateway("rest/register.do", data);
            Response = response;

            JObject JsonDoc = JObject.Parse(response);
            if (JsonDoc["errorCode"].ToString() != "0")
            {
                ErrorMessage = JsonDoc["errorMessage"].ToString();
            }
            else
            {
                PaymentId = JsonDoc["orderId"].ToString();
                PaymentUrl = JsonDoc["formUrl"].ToString();
            }
        }

        public void GetOrderStatus(Dictionary<string, string> data)
        {
            string response = Gateway("rest/getOrderStatus.do", data);
            Response = response;

            JObject JsonDoc = JObject.Parse(response);
            if (JsonDoc["errorCode"].ToString() != "0")
            {
                ErrorMessage = JsonDoc["errorMessage"].ToString();
            }
            else
            {
                OrderStatus = JsonDoc["OrderStatus"].ToString();
            }
        }

        private static string ToQueryString(NameValueCollection queryData)
        {
            var array = (from key in queryData.AllKeys
                         from value in queryData.GetValues(key)
                         select string.Format(CultureInfo.InvariantCulture, "{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }

        private string CombineUrl(string gatewayUrl, string method)
        {
            return new Uri(new Uri(gatewayUrl), method).ToString();
        }
    }
}
