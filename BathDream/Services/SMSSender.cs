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
            string url = $"http://api.sms-prosto.ru/?method=push_msg&key=kv260ab6e63ab1281f671b9c8e1f87ba4963dfd67f1997b9&text={code}&phone={number}&sender_name=BathRemont";

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
