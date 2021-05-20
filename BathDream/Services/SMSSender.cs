using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net;
using System.Collections.Specialized;

namespace BathDream.Services
{
    public class SMSSender
    {
        public int Send(string number, string code)
        {
            string url = $"http://api.sms-prosto.ru/?method=push_msg&key=XEZ60a63f6146d836f5895001ddb3400f912bddba690fdcb&text={code}&phone={number}&sender_name=BathDream";

            using (var webClient = new WebClient())
            {
                var response = webClient.DownloadString(url);
                return GetErrorCode(response);
            }
        }

        public int GetErrorCode(string response)
        {
            XDocument xDoc = XDocument.Parse(response);
            return Convert.ToInt32(xDoc.Element("response").Element("msg").Element("err_code").Value);
        }
    }
}
