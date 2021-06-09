using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace BathDream.Acquiring
{
    public class TinkoffPaymentAPI
    {
        public string ApiUrl { get; set; }
        public string TerminalKey { get; set; }
        public string SecretKey { get; set; }
        public string PaymentId { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
        public string Response { get; set; }
        public string PaymentUrl { get; set; }

        public string Texations { get; } = "usn_income_outcome";
        public string PaymentMethod { get; } = "full_payment";
        public string PaymentObject { get; } = "service";
        public string Vats { get; } = "none";


        public TinkoffPaymentAPI(string terminalKey, string secretKey)
        {
            ApiUrl = "https://securepay.tinkoff.ru/v2/";
            TerminalKey = terminalKey;
            SecretKey = secretKey;
        }
        
        public void Init(Dictionary<string, string> args)
        {
            BuildQuery("Init", args);
        }
        public void GetState(Dictionary<string, string> args)
        {
            BuildQuery("GetState", args);
        }

        private void BuildQuery(string path, Dictionary<string,string> args)
        {
            string url = ApiUrl;
            if (!args.ContainsKey("TerminalKey"))
            {
                args.Add("TerminalKey", TerminalKey);
            }
            if (!args.ContainsKey("Token"))
            {
                args.Add("Token", GenToken(args));
            }

            url = CombineUrl(url, path);

            SendRequest(url, args);
        }

        private void SendRequest(string url, Dictionary<string, string> args)
        {
            string response;
            using (var webClient = new WebClient())
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(args);
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                response = webClient.UploadString(url, json);
            }

            Response = response;

            JObject JsonDoc = JObject.Parse(response);
            if (JsonDoc["ErrorCode"].ToString() != "0")
            {
                Error = JsonDoc["Details"].ToString();
            }
            else
            {
                PaymentUrl = JsonDoc["PaymentUrl"].ToString();
                PaymentId = JsonDoc["PaymentId"].ToString();
                Status = JsonDoc["Status"].ToString();
            }
        }

        private string GenToken(Dictionary<string, string> args)
        {
            string token = "";

            if (!args.ContainsKey("Password"))
            {
                args.Add("Password", SecretKey);
            }

            SortedDictionary<string, string> sortedArgs = new SortedDictionary<string, string>(args);
            foreach (var item in args)
            {
                token += item.Value;
            }

            using (var sha256 = new SHA256Managed())
            {
                token =  BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(token))).Replace("-", "");
            }

            return token;
        }

        private string CombineUrl(string url, string path)
        {
            return new Uri(new Uri(url), path).ToString();
        }

    }
}
